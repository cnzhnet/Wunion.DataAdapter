/***************************************************************************************************************************************
 * 增强型 Label 控件（可定制边框及行间距，支持 AutoSize = false 模式自动换行）
 * 作者：巽翎君
 * 更新日期：2016-4-12
 **************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Wunion.DataAdapter.EntityGenerator.Views
{
    /// <summary>
    /// 增强型 Label 控件（可定制边框及行间距，支持 AutoSize = false 模式时自动换行）。
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [DefaultBindingProperty("Text")]
    [DefaultProperty("Text")]
    public class LingLabel : Label
    {
        private int _LineDistance;
        private bool _AutoSize;
        private bool _DisplayBorder;
        private Color _BorderColor;
        private string _Text;
        private List<LingLabelTextLine> Lines;
        private ContentAlignment _TextAlign;
        private Size ContentSize;
        private bool DisableSizeChangedEvent;
        private bool ResponseRedraw;

        /// <summary>
        /// 创建一个 <see cref="Ling.cnzhnet.Controls.LingLabel"/> 控件的对象实例。
        /// </summary>
        public LingLabel() : base()
        {
            ContentSize = new Size(this.Width, this.Height);
            Lines = new List<LingLabelTextLine>();
            _LineDistance = 0x4;
            base.AutoSize = false;
            _AutoSize = false;
            _DisplayBorder = false;
            _BorderColor = Color.White;
            this.Image = null;
            DisableSizeChangedEvent = false;
            _TextAlign = ContentAlignment.TopLeft;
            Text = "LingLabel1";
        }

        /// <summary>
        /// 创建一个 <see cref="Ling.cnzhnet.Controls.LingLabel"/> 控件的对象实例。
        /// </summary>
        /// <param name="container">承载该 Label 控件的容器。</param>
        public LingLabel(IContainer container) : base()
        {
            container.Add(this);
            ContentSize = new Size(this.Width, this.Height);
            Lines = new List<LingLabelTextLine>();
            _LineDistance = 0x4;
            base.AutoSize = false;
            _AutoSize = false;
            _DisplayBorder = false;
            _BorderColor = Color.White;
            this.Image = null;
            DisableSizeChangedEvent = false;
            _TextAlign = ContentAlignment.TopLeft;
            Text = "LingLabel1";
        }

        /// <summary>
        /// 获取或设置 Label 要显示的文本内容。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置 Label 要显示的文本内容。")]
        public override string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                CalculateLines(true);
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置是否为Label显示边框。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置是否为Label显示边框。")]
        public bool DisplayBorder
        {
            get { return _DisplayBorder; }
            set
            {
                _DisplayBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置 Label 的边框颜色。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置 Label 的边框颜色。")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置行间距。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置行间距。")]
        public int LineDistance
        {
            get { return _LineDistance; }
            set
            {
                _LineDistance = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置控件自动适应内容大小。
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("CatLayout")]
        [Description("获取或设置控件自动适应内容大小。")]
        public new bool AutoSize // 该属性必须隐藏基类的成员，否则当该值为 true 时无法更改 Label 的大小。
        {
            get { return _AutoSize; }
            set
            {
                _AutoSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置文本内容的对齐方式。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置文本内容的对齐方式。")]
        public override ContentAlignment TextAlign
        {
            get { return _TextAlign; }
            set
            {
                _TextAlign = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 向上取整。
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        private int RoundUp(float Val)
        {
            if ((int)Val < Val)
                return (int)Val + 1;
            else
                return (int)Val;
        }

        /// <summary>
        /// 计算 LingLabel 的行。
        /// </summary>
        /// <param name="DoSizeChanged">是否根据需要调用 OnSizeChanged 方法。</param>
        protected void CalculateLines(bool DoSizeChanged)
        {
            GetTextLines(CreateGraphics(), out ContentSize);
            if (AutoSize)
            {
                base.AutoSize = false;
                if (Image != null)
                    this.Size = new Size(ContentSize.Width + Image.Width + Padding.Left + Padding.Right, Math.Max(ContentSize.Height, Image.Height) + Padding.Top + Padding.Bottom);
                else
                    this.Size = new Size(ContentSize.Width + Padding.Left + Padding.Right, ContentSize.Height + Padding.Top + Padding.Bottom);
                if (DoSizeChanged)
                    OnSizeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 从指定的字符串数组组建 Label 应该显示的行，并返回最大行宽。
        /// </summary>
        /// <param name="TextLines">缓冲 Label 行的字符串集合（用于后期绘制）。</param>
        /// <param name="lineArray">以换行符分隔的字符串数组。</param>
        /// <param name="g">用于测量行宽信息的画布对象。</param>
        /// <returns></returns>
        private Size AppendLineFromArray(List<LingLabelTextLine> TextLines, string[] lineArray, Graphics g)
        {
            if (TextLines == null || lineArray == null)
                return new Size(0x0, 0x0);
            if (lineArray.Length < 0x1)
                return new Size(0x0, 0x0);
            SizeF sf = g.MeasureString(lineArray[0], Font);
            TextLines.Add(new LingLabelTextLine(sf, lineArray[0]));
            Size max_size = new Size(RoundUp(sf.Width), RoundUp(sf.Height));
            if (lineArray.Length > 0x1)
            {
                for (int i = 1; i < lineArray.Length; ++i)
                {
                    sf = g.MeasureString(lineArray[i], Font);
                    TextLines.Add(new LingLabelTextLine(sf, lineArray[i]));
                    max_size.Width = Math.Max(max_size.Width, RoundUp(sf.Width));
                }
            }
            return max_size;
        }

        /// <summary>
        /// 获取要绘制的行，并测量出最大行宽值。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="real_size"></param>
        /// <returns></returns>
        private void GetTextLines(Graphics g, out Size real_size)
        {
            Lines = new List<LingLabelTextLine>();
            real_size = new Size(0x0, 0x0);
            string[] lineArray = Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (AutoSize) // 若设计了自动适应大小模式，则直接通过换行符分离出行。
            {
                if (lineArray != null && lineArray.Length > 0)
                    real_size = AppendLineFromArray(Lines, lineArray, g);
                else
                    real_size = AppendLineFromArray(Lines, new string[] { Text }, g);
            }
            else // 若是固定大小模式，则需要根据宽度计算得出行数。
            {
                SizeF lineSize, lineoutSize;
                // 需要判断是否有图标，若指定的图标则文本测量时的宽度限制则应判断视情况减去图标的宽度。
                Size tSize = new Size(this.Width - Padding.Right, this.Height);
                if (Image != null)
                {
                    switch (this.ImageAlign)
                    {
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.TopCenter:
                            tSize = new Size(this.Width - Padding.Right, this.Height);
                            break;
                        default:
                            tSize = new Size(this.Width - Image.Width - Padding.Right, this.Height);
                            break;
                    }
                }
                StringBuilder measure_str;
                string measure_str2;
                int char_index;
                foreach (string line in lineArray)
                {
                    measure_str = new StringBuilder(); // 第一个测量文本（一行的实际文本）。
                    if (Lines.Count > 0x0) // 第一行前面不恢复用户输入的换行。
                        Lines.Add(new LingLabelTextLine(g.MeasureString("\r\n", this.Font), "\r\n"));
                    // 逐字符扫描追加进行测量，直到下一个字符的测量结果超界时判定为一行。
                    for (char_index = 0x0; char_index < line.Length; ++char_index)
                    {
                        measure_str.Append(line[char_index]);
                        // 下一个字符的测量依据。
                        lineSize = g.MeasureString(measure_str.ToString(), Font); // 测量实际的行宽。
                        real_size.Height = (int)(lineSize.Height);
                        if (char_index < (line.Length - 1))
                        {
                            measure_str2 = string.Format("{0}{1}", measure_str, line[char_index + 1]);
                            lineoutSize = g.MeasureString(measure_str2.ToString(), Font); // 测量下一个字符的行宽。
                            if (lineSize.Width <= (tSize.Width) && lineoutSize.Width > tSize.Width)
                            {
                                Lines.Add(new LingLabelTextLine(lineSize, measure_str.ToString()));
                                real_size.Width = Math.Max(real_size.Width, RoundUp(lineSize.Width));
                                // 将已扫描内容判字为一个新行后，应该将测量缓冲重置
                                measure_str = new StringBuilder();
                            }
                        }
                        else // 最后一部分宽度不足宽度超界限制时，应该将其判断为一个行。
                        {
                            Lines.Add(new LingLabelTextLine(lineSize, measure_str.ToString()));
                            real_size.Width = Math.Max(real_size.Width, RoundUp(lineSize.Width));
                            // 将已扫描内容判字为一个新行后，应该将测量缓冲重置
                            measure_str = new StringBuilder();
                        }
                    }
                }
            }
            if (Lines.Count > 0x1)
                real_size.Height = real_size.Height * Lines.Count + (Lines.Count - 1) * LineDistance; // 计算实际高度。
        }

        /// <summary>
        /// 绘制 Label 的边框。
        /// </summary>
        /// <param name="g"></param>
        private void DrawBorder(Graphics g)
        {
            if (!DisplayBorder)
                return;
            g.DrawRectangle(new Pen(BorderColor, 1), new Rectangle(0x0, 0x0, this.Width - 1, this.Height - 1));
        }

        /// <summary>
        /// 绘制 Label 中的图标。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="TextWidth"></param>
        /// <returns></returns>
        private int DrawImage(Graphics g, int TextWidth)
        {
            if (Image == null)
                return Padding.Left;
            PointF pf = new PointF(Padding.Left, Padding.Top);
            const int TEXT_OFFSET = 0x3;
            int textX = Padding.Left;
            switch (this.ImageAlign)
            {
                case ContentAlignment.BottomCenter:
                    pf.X = (this.Width - Image.Width) / 2;
                    pf.Y = Math.Max(pf.Y, this.Height - Image.Height - Padding.Bottom);
                    break;
                case ContentAlignment.BottomLeft:
                    pf.Y = Math.Max(pf.Y, this.Height - Image.Height - Padding.Bottom);
                    textX = RoundUp(pf.X + Image.Width + TEXT_OFFSET);
                    break;
                case ContentAlignment.BottomRight:
                    pf.X = Math.Max(pf.X, this.Width - Image.Width - Padding.Right);
                    pf.Y = Math.Max(pf.Y, this.Height - Image.Height - Padding.Bottom);
                    textX = Padding.Left;
                    break;
                case ContentAlignment.MiddleCenter:
                    pf.X = (this.Width - Image.Width) / 2;
                    pf.Y = (this.Height - Image.Height) / 2;
                    textX = Padding.Left;
                    break;
                case ContentAlignment.MiddleLeft:
                    pf.Y = (this.Height - Image.Height) / 2;
                    textX = RoundUp(pf.X + Image.Width + TEXT_OFFSET);
                    break;
                case ContentAlignment.MiddleRight:
                    pf.X = Math.Max(pf.X, this.Width - Image.Width - Padding.Right);
                    pf.Y = (this.Height - Image.Height) / 2;
                    textX = Padding.Left;
                    break;
                case ContentAlignment.TopCenter:
                    pf.X = Math.Max(pf.X, this.Width - Image.Width - Padding.Right);
                    textX = Padding.Left;
                    break;
                case ContentAlignment.TopLeft:
                    textX = RoundUp(pf.X + Image.Width + TEXT_OFFSET);
                    break;
                case ContentAlignment.TopRight:
                    pf.X = Math.Max(pf.X, this.Width - Image.Width - Padding.Right);
                    textX = Padding.Left;
                    break;
            }
            g.DrawImage(Image, pf);
            return textX;
        }

        /// <summary>
        /// 获得指定行在 X 轴上的绘制起始点。
        /// </summary>
        /// <param name="startX">X轴基础起始点。</param>
        /// <param name="lineWidth">该行的宽度。</param>
        /// <returns></returns>
        private int GetLineDrawStartX(int startX, int lineWidth)
        {
            int textWidth = this.Width - startX;
            switch (this.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    return Math.Max(startX, startX + (textWidth - lineWidth) / 2);
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    return startX;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    return Math.Max(startX, startX + (textWidth - lineWidth - Padding.Right));
            }
            return startX;
        }

        /// <summary>
        /// 获得指定行在 Y 轴上的绘制起始点。
        /// </summary>
        /// <param name="startY">Y轴基础起始点。</param>
        /// <param name="linesSize">所有行的总高度（包含间距）。</param>
        /// <returns></returns>
        private int GetLineDrawStartY(int startY, Size linesSize)
        {
            int textWidth = this.Width - startY;
            switch (this.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    return Math.Max(startY, this.Height - Padding.Bottom - linesSize.Height);
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                    return Math.Max(startY, (this.Height - linesSize.Height) / 0x2);
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    return startY;
            }
            return startY;
        }

        /// <summary>
        /// 绘制 Label 中所要显示的文本。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="DrawLines"></param>
        /// <param name="linesSize"></param>
        /// <param name="startX"></param>
        private void DrawText(Graphics g, Size linesSize, int startX)
        {
            int drawX = GetLineDrawStartX(startX, RoundUp(Lines[0].Size.Width));
            int drawY = GetLineDrawStartY(Padding.Top, linesSize);
            SolidBrush drawBrush = new SolidBrush(this.ForeColor);
            g.DrawString(Lines[0].Text, this.Font, drawBrush, drawX, drawY);
            if (Lines.Count > 0x1)
            {
                for (int i = 0x1; i < Lines.Count; ++i)
                {
                    drawX = GetLineDrawStartX(startX, RoundUp(Lines[i].Size.Width));
                    drawY += Convert.ToInt32(Lines[i].Size.Height) + LineDistance;
                    g.DrawString(Lines[i].Text, this.Font, drawBrush, drawX, drawY);
                }
            }
        }

        /// <summary>
        /// 完全自己绘制 Label 的内容。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                float DpiX = e.Graphics.DpiX / 96;
                float DpiY = e.Graphics.DpiY / 96;
                ResponseRedraw = false;
                Width = (int)(Width * DpiX);
                Height = (int)(Height * DpiY);
                ResponseRedraw = true;
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                //List<LingLabelTextLine> DrawLines = GetTextLines(g, out ContentSize);                
                //g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));
                DrawBorder(g);
                int drawX = DrawImage(g, ContentSize.Width);
                DrawText(g, ContentSize, drawX);

            }
            catch { }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!DisableSizeChangedEvent)
            {
                CalculateLines(false); // 给 Text 再次赋值以重新计算行数及控件大小。
                Invalidate();
            }
            DisableSizeChangedEvent = false;
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// 单独调整 Label 的高度（不触发 OnSizeChanged 重新计算行数且不会重绘）。
        /// </summary>
        /// <param name="height"></param>
        public void ResizeHeightOnly(int height)
        {
            DisableSizeChangedEvent = true;
            this.Height = height;

        }

        /// <summary>
        /// <see cref="Ling.cnzhnet.Controls.LingLabel"/> 控件的行信息对象类型。
        /// </summary>
        private class LingLabelTextLine
        {
            private SizeF _Size;
            private string _Text;

            /// <summary>
            /// 创建一个 <see cref="Ling.cnzhnet.Controls.LingLabel"/> 的对象实例。
            /// </summary>
            /// <param name="size">该行的宽高尺寸。</param>
            /// <param name="text">该行的文本信息。</param>
            public LingLabelTextLine(SizeF size, string text)
            {
                _Size = size;
                _Text = text;
            }

            /// <summary>
            /// 创建一个 <see cref="Ling.cnzhnet.Controls.LingLabel"/> 的对象实例。
            /// </summary>
            /// <param name="width">该行的宽度。</param>
            /// <param name="height">该行的高度。</param>
            /// <param name="text">该行的文本信息。</param>
            public LingLabelTextLine(float width, float height, string text)
            {
                _Size = new SizeF(width, height);
                _Text = text;
            }

            /// <summary>
            /// 获取该行的宽高尺寸。
            /// </summary>
            public SizeF Size
            {
                get { return _Size; }
            }

            /// <summary>
            /// 获取该行的文本信息。
            /// </summary>
            public string Text
            {
                get { return _Text; }
            }
        }
    }
}
