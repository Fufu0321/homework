using System;
using System.Windows.Forms;

namespace CalculatorApp
{
    public class CalculatorForm : Form
    {
        private TextBox displayBox;
        private string currentInput = "";
        private double? firstOperand = null;
        private string pendingOperator = "";
        private bool isNewInput = false;
        private bool hasResult = false;

        public CalculatorForm()
        {
            this.Text = "计算器";
            this.Size = new System.Drawing.Size(300, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // 创建显示文本框
            displayBox = new TextBox
            {
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(260, 30),
                Font = new System.Drawing.Font("微软雅黑", 14, System.Drawing.FontStyle.Regular),
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                Text = "0"
            };
            this.Controls.Add(displayBox);

            // 按钮布局参数
            int btnWidth = 60;
            int btnHeight = 45;
            int margin = 5;
            int startX = 12;
            int startY = 55;

            // 按钮文字和位置（按行排列）
            string[] buttonLabels = new string[]
            {
                "7", "8", "9", "÷",
                "4", "5", "6", "×",
                "1", "2", "3", "-",
                "0", "C", "=", "+"
            };

            for (int i = 0; i < buttonLabels.Length; i++)
            {
                string label = buttonLabels[i];
                int row = i / 4;
                int col = i % 4;

                Button btn = new Button
                {
                    Text = label,
                    Location = new System.Drawing.Point(
                        startX + col * (btnWidth + margin),
                        startY + row * (btnHeight + margin)
                    ),
                    Size = new System.Drawing.Size(btnWidth, btnHeight),
                    Font = new System.Drawing.Font("微软雅黑", 12, System.Drawing.FontStyle.Bold),
                    Tag = label
                };

                // 根据按钮类型设置颜色
                if (label == "C")
                    btn.BackColor = System.Drawing.Color.LightCoral;
                else if (label == "=")
                    btn.BackColor = System.Drawing.Color.LightGreen;
                else if ("+-×÷".Contains(label))
                    btn.BackColor = System.Drawing.Color.LightSkyBlue;
                else
                    btn.BackColor = System.Drawing.Color.WhiteSmoke;

                btn.Click += Button_Click;
                this.Controls.Add(btn);
            }
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            Button? btn = sender as Button;
            if (btn == null) return;

            string text = btn.Text;

            // 清空按钮
            if (text == "C")
            {
                ClearAll();
                return;
            }

            // 数字按钮
            if (char.IsDigit(text[0]))
            {
                if (hasResult || isNewInput)
                {
                    currentInput = "";
                    hasResult = false;
                    isNewInput = false;
                }

                if (currentInput == "0")
                    currentInput = text;
                else
                    currentInput += text;

                displayBox.Text = currentInput;
                return;
            }

            // 运算符按钮 (+, -, ×, ÷)
            if ("+-×÷".Contains(text))
            {
                if (string.IsNullOrEmpty(currentInput))
                    return;

                double currentNum = double.Parse(currentInput);

                if (firstOperand == null)
                {
                    firstOperand = currentNum;
                }
                else if (!isNewInput)
                {
                    firstOperand = Calculate(firstOperand.Value, currentNum, pendingOperator);
                    displayBox.Text = firstOperand.ToString();
                }

                pendingOperator = text;
                isNewInput = true;
                hasResult = false;
                return;
            }

            // 等于按钮
            if (text == "=")
            {
                if (firstOperand == null || string.IsNullOrEmpty(pendingOperator) || string.IsNullOrEmpty(currentInput))
                    return;

                double secondNum = double.Parse(currentInput);
                string expression = $"{firstOperand}{pendingOperator}{secondNum}";
                double result = Calculate(firstOperand.Value, secondNum, pendingOperator);

                displayBox.Text = $"{expression}={result}";

                currentInput = result.ToString();
                firstOperand = null;
                pendingOperator = "";
                hasResult = true;
                isNewInput = true;
            }
        }

        private double Calculate(double a, double b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "×": return a * b;
                case "÷":
                    if (b == 0)
                    {
                        displayBox.Text = "错误：除数不能为0";
                        ClearAll();
                        return 0;
                    }
                    return a / b;
                default:
                    return 0;
            }
        }

        private void ClearAll()
        {
            currentInput = "0";
            firstOperand = null;
            pendingOperator = "";
            isNewInput = false;
            hasResult = false;
            displayBox.Text = "0";
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalculatorForm());
        }
    }
}