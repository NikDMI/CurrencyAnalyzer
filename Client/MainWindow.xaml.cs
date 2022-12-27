using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConfigLibrary;
using Syncfusion.UI.Xaml.Charts;
using System.Text.RegularExpressions;
using Client.Model;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ViewModel(ConfigLibrary.ConfigFactory.GetConfig(ConfigFactory.ConfigType.ASSEMBLY_MEMORY), this);
        }


        //Send dispatch message to redraw graphics
        internal void UpdateGraphicsAsync(List<Rate> rates)
        {
            List<Rate> copyRateList = new List<Rate>(rates);
            chartCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate ()
                        {
                            _linearChart.UpdateRates(copyRateList);
                        }));
        }


        //Show user, that requirest is processing
        internal void StartProcessRequest()
        {
            _requestButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate ()
                        {
                            _requestButton.IsEnabled = false;
                        }));
        }


        //Called at the end of request
        internal void StopProcessRequest()
        {
            _requestButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate ()
                        {
                            _requestButton.IsEnabled = true;
                        }));
        }


        //Chech date validation
        private bool IsValidDateFormat(string format)
        {
            if (Regex.IsMatch(format, @"\d{2}\.\d{2}\.\d{2}\b"))
            {
                try
                {
                    DateTime date = DateTime.Parse(format);
                    if (date > DateTime.Now || (date + TimeSpan.FromDays(MAX_AVAILABLE_DAYS_PERIOD) < DateTime.Now))
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception exp)
                {

                }
            }
            return false;
        }


        private void TextBox_ValidateInput(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string userDate = textBox.Text;
            if (IsValidDateFormat(userDate))
            {
                textBox.Background = Brushes.Gray;
            }
            else
            {
                textBox.Background = Brushes.MediumVioletRed;
            }
        }


        //Send request to the server
        private void SendRequestButton(object sender, RoutedEventArgs e)
        {
            string dateFromText = _textBoxDateFrom.Text;
            string dateToText = _textBoxDateTo.Text;
            if(!IsValidDateFormat(dateFromText) || !IsValidDateFormat(dateToText))
            {
                MessageBox.Show("Некорректный ввод дат (формат дд.мм.гг)");
                return;
            }
            ComboBoxItem currencyItem = _currencyComboBox.SelectedItem as ComboBoxItem;
            if (currencyItem == null)
            {
                MessageBox.Show("Вы не выбрали валюту");
                return;
            }
            DateTime dateFrom = DateTime.Parse(dateFromText);
            DateTime dateTo = DateTime.Parse(dateToText);
            if (dateTo < dateFrom)
            {
                MessageBox.Show("Начальная дата находится дальше конечной даты");
                return;
            }
            _viewModel.UpdateRates(dateFrom, dateTo, ConfigLibrary.Bean.CurrencyType.BYN,
                (ConfigLibrary.Bean.CurrencyType)Enum.Parse(typeof(ConfigLibrary.Bean.CurrencyType), (string)currencyItem.Content));
        }


        private ViewModel _viewModel;

        private const double MAX_AVAILABLE_DAYS_PERIOD = 365.25 * 5;
    }
}
