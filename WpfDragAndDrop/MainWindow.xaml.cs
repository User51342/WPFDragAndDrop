using System.Windows;

namespace WpfDragAndDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private DragDropAnimation myDragDropAni;
        #endregion

        #region Construction / Initialization / Deconstruction
        public MainWindow()
        {
            InitializeComponent();
            myDragDropAni = new DragDropAnimation(RootGrid);
        }
        #endregion

        #region Private implementations
        #endregion
    }
}
