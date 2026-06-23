using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UrlCrawlerApp
{
    public class UrlCrawlerForm : Form
    {
        private TextBox urlTextBox;
        private Button fetchButton;
        private ListBox phoneListBox;
        private ListBox emailListBox;
        private Label statusLabel;
        private Label phoneCountLabel;
        private Label emailCountLabel;
        private TextBox rawContentTextBox;

        public UrlCrawlerForm()
        {
            this.Text = "URL 信息提取器";
            this.Size = new System.Drawing.Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ---- URL 输入区 ----
            Label urlLabel = new Label
            {
                Text = "请输入 URL：",
                Location = new System.Drawing.Point(12, 15),
                Size = new System.Drawing.Size(100, 25),
                Font = new System.Drawing.Font("微软雅黑", 10)
            };

            urlTextBox = new TextBox
            {
                Location = new System.Drawing.Point(110, 12),
                Size = new System.Drawing.Size(420, 25),
                Font = new System.Drawing.Font("微软雅黑", 10),
                Text = "https://example.com"
            };

            fetchButton = new Button
            {
                Text = "获取并提取",
                Location = new System.Drawing.Point(540, 10),
                Size = new System.Drawing.Size(120, 30),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.LightSkyBlue
            };
            fetchButton.Click += FetchButton_Click;

            statusLabel = new Label
            {
                Text = "就绪",
                Location = new System.Drawing.Point(12, 45),
                Size = new System.Drawing.Size(650, 20),
                Font = new System.Drawing.Font("微软雅黑", 9)
            };

            this.Controls.Add(urlLabel);
            this.Controls.Add(urlTextBox);
            this.Controls.Add(fetchButton);
            this.Controls.Add(statusLabel);

            // ---- 结果展示区（左右分栏） ----
            GroupBox phoneGroup = new GroupBox
            {
                Text = "手机号码",
                Location = new System.Drawing.Point(12, 75),
                Size = new System.Drawing.Size(330, 200),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
            };

            phoneListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(310, 135),
                Font = new System.Drawing.Font("Consolas", 10),
                ScrollAlwaysVisible = true
            };

            phoneCountLabel = new Label
            {
                Text = "找到 0 个手机号",
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(310, 20),
                Font = new System.Drawing.Font("微软雅黑", 9)
            };

            phoneGroup.Controls.Add(phoneListBox);
            phoneGroup.Controls.Add(phoneCountLabel);

            GroupBox emailGroup = new GroupBox
            {
                Text = "邮箱地址",
                Location = new System.Drawing.Point(355, 75),
                Size = new System.Drawing.Size(320, 200),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
            };

            emailListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(300, 135),
                Font = new System.Drawing.Font("Consolas", 10),
                ScrollAlwaysVisible = true
            };

            emailCountLabel = new Label
            {
                Text = "找到 0 个邮箱",
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(300, 20),
                Font = new System.Drawing.Font("微软雅黑", 9)
            };

            emailGroup.Controls.Add(emailListBox);
            emailGroup.Controls.Add(emailCountLabel);

            this.Controls.Add(phoneGroup);
            this.Controls.Add(emailGroup);

            // ---- 原始内容展示 ----
            GroupBox rawGroup = new GroupBox
            {
                Text = "页面原始内容（前 1000 字符）",
                Location = new System.Drawing.Point(12, 285),
                Size = new System.Drawing.Size(663, 220),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
            };

            rawContentTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(643, 180),
                Font = new System.Drawing.Font("Consolas", 9),
                ScrollBars = ScrollBars.Vertical,
                Multiline = true,
                ReadOnly = true
            };

            rawGroup.Controls.Add(rawContentTextBox);
            this.Controls.Add(rawGroup);
        }

        private async void FetchButton_Click(object? sender, EventArgs e)
        {
            string url = urlTextBox.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请输入 URL", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 自动补全 http 前缀
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
                urlTextBox.Text = url;
            }

            fetchButton.Enabled = false;
            statusLabel.Text = "正在获取页面内容...";
            phoneListBox.Items.Clear();
            emailListBox.Items.Clear();
            phoneCountLabel.Text = "找到 0 个手机号";
            emailCountLabel.Text = "找到 0 个邮箱";
            rawContentTextBox.Text = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置超时为 10 秒
                    client.Timeout = TimeSpan.FromSeconds(10);
                    // 模拟浏览器 User-Agent，防止被部分网站拒绝
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                    string html = await client.GetStringAsync(url);

                    statusLabel.Text = $"获取成功！共 {html.Length} 个字符";

                    // 显示原始内容前 1000 字符
                    rawContentTextBox.Text = html.Length > 1000
                        ? html.Substring(0, 1000) + "\n\n......（更多内容已省略）"
                        : html;

                    // 提取手机号码（中国大陆手机号：1开头的11位数字）
                    string phonePattern = @"1[3-9]\d{9}";
                    MatchCollection phoneMatches = Regex.Matches(html, phonePattern);
                    HashSet<string> uniquePhones = new HashSet<string>();
                    foreach (Match match in phoneMatches)
                    {
                        uniquePhones.Add(match.Value);
                    }

                    foreach (string phone in uniquePhones)
                    {
                        phoneListBox.Items.Add(phone);
                    }
                    phoneCountLabel.Text = $"找到 {uniquePhones.Count} 个手机号";

                    // 提取邮箱地址
                    string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
                    MatchCollection emailMatches = Regex.Matches(html, emailPattern);
                    HashSet<string> uniqueEmails = new HashSet<string>();
                    foreach (Match match in emailMatches)
                    {
                        uniqueEmails.Add(match.Value);
                    }

                    foreach (string email in uniqueEmails)
                    {
                        emailListBox.Items.Add(email);
                    }
                    emailCountLabel.Text = $"找到 {uniqueEmails.Count} 个邮箱";
                }
            }
            catch (HttpRequestException ex)
            {
                statusLabel.Text = "网络请求失败";
                MessageBox.Show($"无法访问该 URL：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                statusLabel.Text = "请求超时";
                MessageBox.Show("请求超时，请检查 URL 是否正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "发生错误";
                MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                fetchButton.Enabled = true;
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UrlCrawlerForm());
        }
    }
}