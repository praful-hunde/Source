// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot;

namespace SimpleDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            var actualController = this.plot.ActualController;
            var controller = new PlotController();
            controller.BindMouseDown(OxyMouseButton.Left , OxyModifierKeys.Control, PlotCommands.ZoomRectangle);
            controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
          
            this.plot.Controller = controller;
           
        }
   }
}