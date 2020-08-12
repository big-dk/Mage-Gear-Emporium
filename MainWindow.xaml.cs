using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using z3test;

namespace Mage_Gear_Emporium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Result result { get; set; }
        Result resultStaff { get; set; }
        Result result1h { get; set; }
        System.Diagnostics.Stopwatch stopwatch { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // exclude selected item
            foreach (var cell in this.resultsTable.SelectedCells)
            {
                if (cell.Column.DisplayIndex == 0)
                {
                    ResultItem item = (ResultItem)cell.Item;
                    ExcludedIdList.Text = $"{ExcludedIdList.Text}{item.itemid}\n";
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // calculate

            // FIXME: add a button to open in wowhead.

            HashSet<int> excludedItems = new HashSet<int>();
            HashSet<int> requireUse = new HashSet<int>();

            if (this.hasr14pvp.IsChecked != true)
            {
                excludedItems.Add(23451);
                excludedItems.Add(23466);
            }
            if (this.has10pvp.IsChecked != true)
            {
                excludedItems.Add(23319);
            }

            foreach (var i in ExcludedIdList.Text.Split("\n"))
            {
                if (int.TryParse(i.Replace(",", "").Trim(), out var x))
                {
                    excludedItems.Add(x);
                }
            }

            foreach (var i in IncludeList.Text.Split("\n"))
            {
                if (int.TryParse(i.Replace(",", "").Trim(), out var x))
                {
                    requireUse.Add(x);
                }
            }

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            progress.Visibility = Visibility.Visible;
            resultStaff = null;
            result1h = null;

            bool listModeIsExclude = excludeListMode.IsChecked == true;

            double critval = double.Parse(this.OneCritEquals.Text);

            new Thread(() =>
            {
                resultStaff = z3test.Program.Calculate(critval, true, excludedItems, listModeIsExclude, requireUse);

                this.Dispatcher.Invoke(() =>
                {
                    updateUI();
                });
                
            }).Start();
            new Thread(() =>
            {
                result1h = z3test.Program.Calculate(critval, false, excludedItems, listModeIsExclude, requireUse);
                this.Dispatcher.Invoke(() =>
                {
                    updateUI();
                });

            }).Start();

        }

        private void updateUI()
        {
            if (resultStaff != null && result1h != null)
            {
                progress.Visibility = Visibility.Hidden;
                stopwatch.Stop();
                debugstats.Content = $"Time taken: {stopwatch.Elapsed}";
                if (resultStaff.equivSp > result1h.equivSp)
                {
                    result = resultStaff;
                }
                else
                {
                    result = result1h;
                }

                resultsTable.ItemsSource = result.items;

                if (result.items.Count == 0)
                {
                    resultSummary.Text = "There is no solution. You may not of provided enough options to make a full set of gear.";
                } else
                {
                    resultSummary.Text = $"eqsp: {result.equivSp}, raw_sp: {result.rawSp}, raw_hit: {result.hit}, raw_crit: {result.crit}";

                }


            }

        }

        private void onlyPickItems_Checked(object sender, RoutedEventArgs e)
        {
            itemListTitle.Content = "Use only these items";
        }

        private void excludeListMode_Checked(object sender, RoutedEventArgs e)
        {
            itemListTitle.Content = "Exclude these items";

        }
    }
}
