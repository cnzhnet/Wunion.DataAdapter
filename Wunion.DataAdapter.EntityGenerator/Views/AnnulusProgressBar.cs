/***************************************************************************************************************************************
 * 环状进度条控件。
 * 作者：巽翎君
 * 更新日期：2016-4-11
 **************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;


namespace Wunion.DataAdapter.EntityGenerator.Views
{
    /// <summary>
    /// 环状进度件控件。
    /// </summary>
    public class AnnulusProgressBar : Control
    {
        private float _ProgressValue;
        private bool _DisplayProgressText;
        private bool DPI_Resized;
        private bool ResponseRedraw;
        private Color _WideColor;
        private Color _ProgressColor;
        /// <summary>
        /// 圆心的坐标位置。
        /// </summary>
        private int PointCenter;
        private float _ZoneWide;

        /// <summary>
        /// 创建一个 <see cref="Ling.cnzhnet.Controls.AnnulusProgressBar"/> 环状进度条。
        /// </summary>
        public AnnulusProgressBar()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Size = new Size(100, 100);
            _ProgressValue = 0f;
            _DisplayProgressText = true;
            DPI_Resized = false;
            _ZoneWide = 0x3;
            BackColor = Color.Transparent;
            _WideColor = Color.FromArgb(39, 109, 239);
            _ProgressColor = Color.Purple;
            ForeColor = Color.Purple;
            ResponseRedraw = true;
        }

        /// <summary>
        /// 获取或设置进度条的百分比值（0 到 1.0 之间）。
        /// </summary>
        [Browsable(true), Category("Action"), Description("获取或设置进度条的百分比值（0 到 1.0 之间）。")]
        public float ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                if (value > 1.0f)
                    throw new Exception("百分比值超过限制。必须是 0 到 1.0 之间的浮点数！");
                _ProgressValue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置是否显示百分比文本信息（将在环形的中心显示）。
        /// </summary>
        [Browsable(true), Category("Action"), Description("获取或设置是否显示百分比文本信息（将在环形的中心显示）。")]
        public bool DisplayProgressText
        {
            get { return _DisplayProgressText; }
            set
            {
                _DisplayProgressText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置用于体现百分比的环带的粗细。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置用于体现百分比的环带的粗细。")]
        public float ZoneWide
        {
            get { return _ZoneWide; }
            set
            {
                _ZoneWide = Math.Max(value, 1.0f); // 环带的粗细必须大于0
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置环带的颜色。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置环带的颜色。")]
        public Color WideColor
        {
            get { return _WideColor; }
            set
            {
                _WideColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置已完成进度的颜色。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置已完成进度的颜色。")]
        public Color ProgressColor
        {
            get { return _ProgressColor; }
            set
            {
                _ProgressColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置按钮背景颜色。
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("获取或设置按钮背景颜色。")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 绘制进度条控件的展示内容。
        /// </summary>
        /// <param name="g"></param>
        protected void DrawContent(Graphics g)
        {
            const int FULL_HE = 360; // 满弧弧度。
            const int hS = 270; // 进度弧度的起始位置。
            // 圆心的坐标点。
            PointCenter = (int)(ZoneWide / 2);
            // 圆圈大小（取宽和高中小的一个减去环带宽度为准，避免圆圈画出界）
            int OutsideSize = Math.Min(Width, Height) - (int)ZoneWide - PointCenter;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(new Pen(WideColor, ZoneWide), PointCenter, PointCenter, OutsideSize, OutsideSize);

            // OK,接下来要开始画已完成进度的弧度。
            float hE = FULL_HE * ProgressValue; // 计算已完成的弧度。
            int i = 0x0;
            for (; i < 0x2; ++i) // 画两遍增强显示效果。
            {
                g.DrawArc(new Pen(ProgressColor, ZoneWide),
                    PointCenter, PointCenter, OutsideSize, OutsideSize, hS, hE);
            }

            // 接下来绘制文本显示的百分比信息。
            if (DisplayProgressText)
            {
                string progressText = string.Format("{0}%", Math.Round(ProgressValue * 100, 0));
                StringFormat sf = new StringFormat();
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                SizeF size_f = g.MeasureString(progressText, Font, 1024, sf);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                for (i = 0; i < 0x2; ++i) // 画两遍增强显示效果。
                {
                    g.DrawString(progressText, Font, new SolidBrush(ForeColor),
                                 (OutsideSize - size_f.Width) / 2 + ZoneWide,
                                 (OutsideSize - size_f.Height) / 2 + ZoneWide);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!ResponseRedraw)
                return;
            //if (!DPI_Resized)
            //{
            //    float DpiX = e.Graphics.DpiX / 96;
            //    float DpiY = e.Graphics.DpiY / 96;
            //    ResponseRedraw = false;
            //    Width = (int)(Width * DpiX);
            //    Height = (int)(Height * DpiY);
            //    ResponseRedraw = true;
            //    DPI_Resized = true;
            //}
            DrawContent(e.Graphics);
        }
    }
}
