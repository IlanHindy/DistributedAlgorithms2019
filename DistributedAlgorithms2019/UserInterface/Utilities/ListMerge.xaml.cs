using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DistributedAlgorithms
{
    /// <summary>
    /// Interaction logic for ListMerge.xaml
    /// </summary>
    public partial class ListMerge : Window
    {
        private class PresentData
        {
            public int indexInExsist;
            public int indexInNew;
            public Attribute attribute;
            public PresentData(Attribute attribute, int indexInExsist, int indexInNew = -1)
            {
                this.attribute = attribute;
                this.indexInExsist = indexInExsist;
                this.indexInNew = indexInNew;
            }
        }

        AttributeList existList;
        AttributeList newList;
        List<PresentData> existPresentation = new List<PresentData>();
        List<PresentData> newPresentation = new List<PresentData>();
        public ListMerge(AttributeList existList, AttributeList newList)
        {
            InitializeComponent();
            this.existList = existList;
            this.newList = newList;
            FillPresentData();
            PresentAll();
        }

        private void FillPresentData()
        { 
            for (int idx = 0; idx < existList.Count; idx ++)
            {
                existPresentation.Add(new PresentData(existList.GetAttribute(idx), idx));
            }

            for (int idx = 0; idx < newList.Count; idx++)
            {
                newPresentation.Add(new PresentData(newList.GetAttribute(idx), -1, idx));
            }
        }

        private void PresentAll()
        {
            foreach (PresentData presentData in existPresentation)
            {
                StackPanet_Exists.Children.Add(PresentOne(presentData, false));
            }

            foreach (PresentData presentData in newPresentation)
            {
                StackPanet_New.Children.Add(PresentOne(presentData, true));
            }
        }

        private Grid PresentOne(PresentData presentData, bool enable)
        {
            Grid grid = new Grid();
            grid.Height = 25;
            grid.Margin = new Thickness(5);
            ColumnDefinition c0 = new ColumnDefinition();
            c0.Width = new GridLength(15, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c0);
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(15, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c1);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(100, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c2);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(15, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c3);
            ColumnDefinition c4 = new ColumnDefinition();
            c4.Width = new GridLength(15, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c4);


            TextBlock indexInExsist = new TextBlock() { Text = presentData.indexInExsist.ToString(), IsEnabled = enable};
            grid.Children.Add(indexInExsist);
            Grid.SetColumn(indexInExsist, 0);
            TextBlock indexInNew = new TextBlock() { Text = presentData.indexInNew.ToString(), IsEnabled = enable };
            grid.Children.Add(indexInNew);
            Grid.SetColumn(indexInNew, 1);
            TextBlock value = new TextBlock() { Text = presentData.attribute.GetValueToString(), IsEnabled = enable };
            grid.Children.Add(value);
            Grid.SetColumn(value, 2);
            CheckBox edittable = new CheckBox() { IsChecked = presentData.attribute.Editable, IsEnabled = enable };
            grid.Children.Add(edittable);
            Grid.SetColumn(edittable, 3);
            return grid;
        }
    }
}
