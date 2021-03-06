﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110215
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20100215
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Rafy.MetaModel.View;
using Rafy.WPF;

namespace Rafy.WPF
{
    /// <summary>
    /// 一个生成的控件结果
    /// 包含一个控件及其对应的逻辑视图。
    /// </summary>
    [DebuggerDisplay("{Control.DependencyObjectType.Name} View:{MainView.EntityType.Name}")]
    public class ControlResult
    {
        public ControlResult(FrameworkElement control, LogicalView view)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (view == null) throw new ArgumentNullException("control");

            this.Control = control;
            this.MainView = view;
        }

        public ControlResult(FrameworkElement control, LogicalView view, AggtBlocks blocks)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (view == null) throw new ArgumentNullException("control");

            this.Control = control;
            this.MainView = view;
            this.Blocks = blocks;
        }

        /// <summary>
        /// 聚合控件块
        /// </summary>
        public FrameworkElement Control { get; private set; }

        /// <summary>
        /// 对应的逻辑主视图
        /// </summary>
        public LogicalView MainView { get; private set; }

        /// <summary>
        /// 本聚合控件对应的聚合块定义
        /// </summary>
        public AggtBlocks Blocks { get; private set; }

        /// <summary>
        /// 一个简单的块也可以直接转换为一个聚合块。
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static implicit operator ControlResult(LogicalView view)
        {
            return new ControlResult(view.Control, view);
        }
    }
}