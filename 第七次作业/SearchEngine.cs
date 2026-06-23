using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngineApp
{
    public class SearchForm : Form
    {
        private TextBox keywordTextBox;
        private Button searchButton;
        private TextBox baiduResultBox;
        private TextBox bingResultBox;
        private Label statusLabel;
        private Label baiduTimeLabel;
        private Label bingTimeLabel;

        public SearchForm()
        {
            this.Text = "搜索引擎关键字搜索";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ---- 顶部输入区 ----
            Label keywordLabel = new Label
            {
                Text = "输入关键字：",
                Location = new System.Drawing.Point(12, 15),
                Size = new System.Drawing.Size(90, 25),
                Font = new System.Drawing.Font("微软雅黑", 10)
            };

            keywordTextBox = new TextBox
            {
                Location = new System.Drawing.Point(105, 12),
                Size = new System.Drawing.Size(450, 25),
                Font = new System.Drawing.Font("微软雅黑", 10)
            };

            searchButton = new Button
            {
                Text = "并行搜索",
                Location = new System.Drawing.Point(565, 10),
                Size = new System.Drawing.Size(100, 30),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.LightSkyBlue
            };
            searchButton.Click += SearchButton_Click;

            statusLabel = new Label
            {
                Text = "就绪",
                Location = new System.Drawing.Point(12, 45),
                Size = new System.Drawing.Size(760, 20),
                Font = new System.Drawing.Font("微软雅黑", 9),
                ForeColor = System.Drawing.Color.Gray
            };

            this.Controls.Add(keywordLabel);
            this.Controls.Add(keywordTextBox);
            this.Controls.Add(searchButton);
            this.Controls.Add(statusLabel);

            // ---- 百度结果区域 ----
            GroupBox baiduGroup = new GroupBox
            {
                Text = "百度搜索结果（前200字）",
                Location = new System.Drawing.Point(12, 75),
                Size = new System.Drawing.Size(380, 240),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
            };

            baiduTimeLabel = new Label
            {
                Text = "等待搜索...",
                Location = new System.Drawing.Point(10, 22),
                Size = new System.Drawing.Size(360, 18),
                Font = new System.Drawing.Font("微软雅黑", 8),
                ForeColor = System.Drawing.Color.Gray
            };

            baiduResultBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 43),
                Size = new System.Drawing.Size(360, 185),
                Font = new System.Drawing.Font("微软雅黑", 9),
                ScrollBars = ScrollBars.Vertical,
                Multiline = true,
                ReadOnly = true
            };

            baiduGroup.Controls.Add(baiduTimeLabel);
            baiduGroup.Controls.Add(baiduResultBox);

            // ---- 必应结果区域 ----
            GroupBox bingGroup = new GroupBox
            {
                Text = "必应搜索结果（前200字）",
                Location = new System.Drawing.Point(400, 75),
                Size = new System.Drawing.Size(380, 240),
                Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
            };

            bingTimeLabel = new Label
            {
                Text = "等待搜索...",
                Location = new System.Drawing.Point(10, 22),
                Size = new System.Drawing.Size(360, 18),
                Font = new System.Drawing.Font("微软雅黑", 8),
                ForeColor = System.Drawing.Color.Gray
            };

            bingResultBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 43),
                Size = new System.Drawing.Size(360, 185),
                Font = new System.Drawing.Font("微软雅黑", 9),
                ScrollBars = ScrollBars.Vertical,
                Multiline = true,
                ReadOnly = true
            };

            bingGroup.Controls.Add(bingTimeLabel);
            bingGroup.Controls.Add(bingResultBox);

            this.Controls.Add(baiduGroup);
            this.Controls.Add(bingGroup);

            // ---- 下方说明 ----
            Label infoLabel = new Label
            {
                Text = "说明：点击「并行搜索」后，会同时（异步）请求百度和必应搜索引擎，\n"
                     + "从搜索结果页面提取纯文字内容的前200个字。\n"
                     + "使用了 async/await 异步编程 + Task.WhenAll 并行搜索。",
                Location = new System.Drawing.Point(12, 330),
                Size = new System.Drawing.Size(760, 60),
                Font = new System.Drawing.Font("微软雅黑", 9),
                ForeColor = System.Drawing.Color.Gray
            };
            this.Controls.Add(infoLabel);
        }

        private async void SearchButton_Click(object? sender, EventArgs e)
        {
            string keyword = keywordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("请输入搜索关键字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 清空之前结果
            baiduResultBox.Text = "正在搜索...";
            bingResultBox.Text = "正在搜索...";
            baiduTimeLabel.Text = "搜索中...";
            bingTimeLabel.Text = "搜索中...";
            searchButton.Enabled = false;
            statusLabel.Text = $"正在并行搜索 \"{keyword}\" ...";
            statusLabel.ForeColor = System.Drawing.Color.Blue;

            try
            {
                string encodedKeyword = Uri.EscapeDataString(keyword);

                // 创建两个异步搜索任务（并行执行）
                Task<SearchResult> baiduTask = SearchBaiduAsync(encodedKeyword);
                Task<SearchResult> bingTask = SearchBingAsync(encodedKeyword);

                // 并行等待两个任务全部完成
                await Task.WhenAll(baiduTask, bingTask);

                SearchResult baiduResult = baiduTask.Result;
                SearchResult bingResult = bingTask.Result;

                // 截取前200个字并显示
                baiduResultBox.Text = TruncateToNChars(baiduResult.PlainText, 200);
                bingResultBox.Text = TruncateToNChars(bingResult.PlainText, 200);

                baiduTimeLabel.Text = $"搜索耗时：{baiduResult.ElapsedMs}ms";
                bingTimeLabel.Text = $"搜索耗时：{bingResult.ElapsedMs}ms";

                statusLabel.Text = $"搜索完成！百度：{baiduResult.PlainText.Length}字，必应：{bingResult.PlainText.Length}字";
                statusLabel.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "搜索出错";
                statusLabel.ForeColor = System.Drawing.Color.Red;
                if (string.IsNullOrEmpty(baiduResultBox.Text) || baiduResultBox.Text == "正在搜索...")
                    baiduResultBox.Text = $"百度搜索失败：{ex.Message}";
                if (string.IsNullOrEmpty(bingResultBox.Text) || bingResultBox.Text == "正在搜索...")
                    bingResultBox.Text = $"必应搜索失败：{ex.Message}";
            }
            finally
            {
                searchButton.Enabled = true;
            }
        }

        /// <summary>
        /// 异步搜索百度
        /// </summary>
        private async Task<SearchResult> SearchBaiduAsync(string encodedKeyword)
        {
            Stopwatch sw = Stopwatch.StartNew();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");

                string url = $"https://www.baidu.com/s?wd={encodedKeyword}";
                string html = await client.GetStringAsync(url);

                sw.Stop();
                string plainText = ExtractPlainText(html);
                return new SearchResult(plainText, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 异步搜索必应
        /// </summary>
        private async Task<SearchResult> SearchBingAsync(string encodedKeyword)
        {
            Stopwatch sw = Stopwatch.StartNew();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");

                string url = $"https://www.bing.com/search?q={encodedKeyword}";
                string html = await client.GetStringAsync(url);

                sw.Stop();
                string plainText = ExtractPlainText(html);
                return new SearchResult(plainText, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 从 HTML 中提取纯文本
        /// </summary>
        private string ExtractPlainText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            string text = html;

            // 移除 script 和 style 标签及其内容
            text = Regex.Replace(text, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<style[^>]*>[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<noscript[^>]*>[\s\S]*?</noscript>", "", RegexOptions.IgnoreCase);

            // 移除所有 HTML 标签
            text = Regex.Replace(text, @"<[^>]+>", " ");

            // 解码 HTML 实体
            text = System.Net.WebUtility.HtmlDecode(text);

            // 替换连续空白为一个空格
            text = Regex.Replace(text, @"\s+", " ");

            // 移除 &nbsp; 等残留实体
            text = Regex.Replace(text, @"&[a-z]+;", " ");

            // 移除特殊 Unicode 字符
            text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

            return text.Trim();
        }

        /// <summary>
        /// 截取前 N 个字符
        /// </summary>
        private string TruncateToNChars(string text, int maxCount)
        {
            if (string.IsNullOrEmpty(text))
                return "（无内容）";

            if (text.Length <= maxCount)
                return text;

            return text.Substring(0, maxCount) + "\n\n......（已截断，仅显示前 " + maxCount + " 个字）";
        }
    }

    /// <summary>
    /// 搜索结果数据类
    /// </summary>
    public class SearchResult
    {
        public string PlainText { get; }
        public long ElapsedMs { get; }

        public SearchResult(string plainText, long elapsedMs)
        {
            PlainText = plainText;
            ElapsedMs = elapsedMs;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SearchForm());
        }
    }
}