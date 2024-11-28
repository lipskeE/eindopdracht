using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace eind_opdracht
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        byte[] _data;
        DispatcherTimer _dispatcherTimer;
        const int NUMBER_OF_DMX_BYTES = 513;
        int kanaal;
        int kanaal2;
        
        public MainWindow()
        {
            InitializeComponent();
            _cts = new CancellationTokenSource();
            foreach (string s in SerialPort.GetPortNames())
                Port.Items.Add(s);

            _serialPort = new SerialPort();
            _serialPort.BaudRate = 250000;
            _serialPort.StopBits = StopBits.Two;

            _data = new byte[NUMBER_OF_DMX_BYTES];
            

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(0.025);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
            manual.Visibility = Visibility.Hidden;
            sl8.Value = 253;
            slidersp.Value =0.5;
            slidersp.Minimum = 0.5;
            sl11.Visibility = Visibility.Hidden;
            kleur1.Visibility = Visibility.Hidden;
            next1.Visibility = Visibility.Hidden;
            sl12.Visibility = Visibility.Hidden;
            dimmer2.Visibility = Visibility.Hidden;
            sl13.Visibility = Visibility.Hidden;
            pan1.Visibility = Visibility.Hidden;
            sl14.Visibility = Visibility.Hidden;
            tilt1.Visibility = Visibility.Hidden;
            dmx2.Visibility = Visibility.Hidden;
            dmxtextbox1.Visibility = Visibility.Hidden;
            kanaalig2.Visibility = Visibility.Hidden;
            startk.Visibility = Visibility.Hidden;
            split1.Visibility = Visibility.Hidden;
            split2.Visibility = Visibility.Hidden;
            split3.Visibility = Visibility.Hidden;
            split.Visibility = Visibility.Hidden;
            var margin = kanaalig.Margin;
            kanaalig.Margin = new Thickness(margin.Left, margin.Top - 26, margin.Right, margin.Bottom);
        }
        private void _dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            SendDmxData(_data, _serialPort);
        }
        private void SendDmxData(byte[] data, SerialPort serialPort)
        {
            data[0] = 0;

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.BreakState = true;
                Thread.Sleep(1);
                serialPort.BreakState = false;
                Thread.Sleep(1);
                serialPort.Write(data, 0, data.Length);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SendDmxData(new byte[NUMBER_OF_DMX_BYTES], _serialPort);
            _serialPort.Dispose();
        }
        private void Port_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                if (Port.SelectedItem.ToString() != "select com-poort")
                {
                    try
                    {
                        _serialPort.PortName = Port.SelectedItem.ToString();
                        _serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fout bij het openen van de poort: {ex.Message}\nSelecteer een andere COM-poort.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selecteer een geldige COM-poort.", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void dmxtextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }
        private bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }
        private void dmxtextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(dmxtextbox.Text, out int value))
            {
                if (value > 512)
                {
                    MessageBox.Show("DMX gaat tot 512", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dmxtextbox.Text = "512";
                    dmxtextbox.SelectionStart = dmxtextbox.Text.Length;
                }
            }
            else if (!string.IsNullOrEmpty(dmxtextbox.Text))
            {
                dmxtextbox.Text = "";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(dmxtextbox.Text, out int newAddress))
                kanaal = newAddress;
            kanaalig.Content = "kanaal in gebruik: " + kanaal;
        }
        private void dmxtextbox_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric1(e.Text);
        }
        private bool IsTextNumeric1(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }
        private void dmxtextbox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(dmxtextbox1.Text, out int value))
            {
                if (value > 512)
                {
                    MessageBox.Show("DMX gaat tot 512", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dmxtextbox1.Text = "512";
                    dmxtextbox1.SelectionStart = dmxtextbox1.Text.Length;
                }
            }
            else if (!string.IsNullOrEmpty(dmxtextbox1.Text))
            {
                dmxtextbox1.Text = "";
            }
        }
        private void Button2(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(dmxtextbox1.Text, out int newAddress))
                kanaal2 = newAddress;
            kanaalig2.Content = "kanaal2 in gebruik: " + kanaal2;
        }
        //DIMMEN---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dimmer.Content = "dim=" + Math.Round(sl1.Value / 2.55, 0) + "%";
            _data[kanaal + 7] = Convert.ToByte(sl1.Value);
            _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);
            if (sl12.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
            }
            else
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);
                sl12.Value=sl1.Value;
            }
        }
        private void btn1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 7] = Convert.ToByte(255);
            _data[kanaal2 + 7] = Convert.ToByte(255);
            dimmer.Content = "dim=100%";
            dimmer2.Content= "dim=100%";
        }
        private void btn1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 7] = Convert.ToByte(sl1.Value);
            _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
            dimmer.Content = "dim=" + Math.Round(sl1.Value / 2.55, 0) + "%";
            dimmer2.Content = "dim=" + Math.Round(sl12.Value / 2.55, 0) + "%";
        }
        private void split1_Checked(object sender, RoutedEventArgs e)
        {
            if (sl1 != null)
            {
                var margin = sl1.Margin;
                var margin1= dimmer.Margin;
                sl1.Margin = new Thickness(margin.Left - 17.5, margin.Top, margin.Right - 17.5, margin.Bottom);
                dimmer.Margin = new Thickness(margin1.Left - 17.5, margin1.Top, margin1.Right - 17.5, margin1.Bottom);
                sl12.Visibility = Visibility.Visible;
                dimmer2.Visibility = Visibility.Visible;
            }
            _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
        }
        private void split1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sl1 != null)
            {
                var margin = sl1.Margin;
                var margin1= dimmer.Margin;
                sl1.Margin = new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                dimmer.Margin = new Thickness(margin1.Left + 17.5, margin1.Top, margin.Right + 17.5, margin.Bottom);
                sl12.Visibility = Visibility.Hidden;
                dimmer2 .Visibility = Visibility.Hidden;
                sl11.Value = sl1.Value;
            }
        }
        private void sl12_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dimmer2.Content = "dim2=" + Math.Round(sl12.Value / 2.55, 0) + "%";

            if (sl12.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
            }
            else
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);
                sl11.Value = sl1.Value;
            }
        }
        //PAN---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void split2_Checked(object sender, RoutedEventArgs e)
        {
            sl13.Visibility = Visibility.Visible;
            pan1.Visibility = Visibility.Visible;
            if (sl2 != null)
            {
                var margin = sl2.Margin;
                var margin2 = pan.Margin;
                sl2.Margin = new Thickness(margin.Left - 17.5, margin.Top, margin.Right - 17.5, margin.Bottom);
                pan.Margin = new Thickness(margin2.Left - 17.5, margin2.Top, margin2.Right - 17.5, margin2.Bottom);
            }
            _data[kanaal2 + 0] = Convert.ToByte(sl13.Value);
        }
        private void split2_Unchecked(object sender, RoutedEventArgs e)
        {
            sl13.Visibility = Visibility.Hidden;
            pan1.Visibility = Visibility.Hidden;
            if (sl2 != null)
            {
                var margin = sl2.Margin;
                var margin2 = pan.Margin;
                sl2.Margin = new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                pan.Margin = new Thickness(margin2.Left + 17.5, margin2.Top, margin2.Right + 17.5, margin2.Bottom);
            }
        }
        private void sl13_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pan1.Content = "pan2 " + Math.Round(sl13.Value * 2.1176470, 0) + "°";

            if (sl13.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl13.Value);
            }
            else
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
                sl13.Value = sl2.Value;
            }
        }
        private void sl2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 0] = Convert.ToByte(sl2.Value);
            _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
            pan.Content = "pan " + Math.Round(sl2.Value * 2.1176470, 0) + "°";
            if (sl13.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl13.Value);
            }
            else
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
                sl13.Value=sl2.Value;
            }
        }
        //TITLT---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 2] = Convert.ToByte(sl3.Value);
            _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
            tilt.Content = "tilt " + Math.Round(sl3.Value * 0.8039215, 0) + "°";
            if (sl14.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);
            }
            else
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
                sl14.Value = sl3.Value;
            }
        }
        private void split3_Checked(object sender, RoutedEventArgs e)
        {

            sl14.Visibility = Visibility.Visible;
            tilt1.Visibility = Visibility.Visible;
            if (sl3 != null)
            {
                var margin = sl3.Margin;
                var margin1 = tilt.Margin;
                sl3.Margin = new Thickness(margin.Left - 17.5, margin.Top, margin.Right - 17.5, margin.Bottom);
                tilt.Margin = new Thickness(margin1.Left - 17.5, margin1.Top, margin1.Right - 17.5, margin1.Bottom);
            }
            _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);
        }
        private void split3_Unchecked(object sender, RoutedEventArgs e)
        {
            sl14.Visibility = Visibility.Hidden;
            tilt1.Visibility= Visibility.Hidden;
            if (sl3 != null)
            {
                var margin = sl3.Margin;
                var margin1= tilt.Margin;
                sl3.Margin= new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                tilt.Margin = new Thickness(margin1.Left + 17.5, margin1.Top, margin1.Right + 17.5, margin1.Bottom);
            }
        }
        private void sl14_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tilt1.Content = "tilt2 " + Math.Round(sl14.Value * 0.8039215, 0) + "°";
            if (sl14.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);
            }
            else
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
                sl14.Value = sl3.Value;
            }
        }
        //STROBO---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strobo.Content = "strobo=" + sl4.Value / 2 + "%";
            _data[kanaal + 6] = Convert.ToByte(sl4.Value);
            _data[kanaal2 + 6] = Convert.ToByte(sl4.Value);
        }
        private void btn4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sl4.Value == 0)
            {
                _data[kanaal + 6] = Convert.ToByte(200);
                _data[kanaal2 + 6] = Convert.ToByte(200);
                strobo.Content = "strobo=100%";
            }
        }
        private void btn4_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_data[kanaal + 6] == 200)
            {
                _data[kanaal + 6] = Convert.ToByte(0);
                _data[kanaal2 + 6] = Convert.ToByte(0);
                strobo.Content = "strobo=0%";
            }
        }
        //KLEUR---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl5_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (sl5.Value)
            {
                case 0:
                    kleur.Content = "wit";
                    next.Content = "next: rd";
                    break;
                case 10:
                    kleur.Content = "rood";
                    next.Content = "next: or";
                    break;
                case 20:
                    kleur.Content = "oranje";
                    next.Content = "next: lgr";
                    break;
                case 30:
                    kleur.Content = "l groen";
                    next.Content = "next: dgr";
                    break;
                case 40:
                    kleur.Content = "d groen";
                    next.Content = "next: bl";
                    break;
                case 50:
                    kleur.Content = "blauw";
                    next.Content = "next: re";
                    break;
                case 60:
                    kleur.Content = "roze";
                    next.Content = "next: lbl";
                    break;
                case 70:
                    kleur.Content = "l blauw";
                    next.Content = "end";
                    break;
            }
            _data[kanaal + 4] = Convert.ToByte(sl5.Value);
            if (sl11.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
            }
            else
            {
                _data[kanaal2 + 4] = Convert.ToByte(sl5.Value);
                sl11.Value = sl5.Value;
            }
        }
        private void split_Checked(object sender, RoutedEventArgs e)
        {
            if (sl5 != null)
            {
                var margin = sl5.Margin;
                var margin1 = kleur.Margin;
                var margin2 = next.Margin;
                sl5.Margin = new Thickness(margin.Left - 17.5, margin.Top, margin.Right - 17.5, margin.Bottom);
                kleur.Margin = new Thickness(margin1.Left - 17.5, margin1.Top, margin1.Right - 17.5, margin1.Bottom);
                next.Margin = new Thickness(margin2.Left - 17.5, margin2.Top, margin2.Right - 17.5, margin2.Bottom);
                sl11.Visibility = Visibility.Visible;
                kleur1.Visibility = Visibility.Visible;
                next1.Visibility = Visibility.Visible;
            }
            _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
        }
        private void split_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sl5 != null)
            {
                var margin = sl5.Margin;
                var margin1 = kleur.Margin;
                var margin2 = next.Margin;
                sl5.Margin = new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                kleur.Margin = new Thickness(margin1.Left + 17.5, margin1.Top, margin1.Right + 17.5, margin1.Bottom);
                next.Margin = new Thickness(margin2.Left + 17.5, margin2.Top, margin2.Right + 17.5, margin2.Bottom);
                sl11.Visibility = Visibility.Hidden;
                kleur1.Visibility = Visibility.Hidden;
                next1.Visibility = Visibility.Hidden;
            }
        }
        private void sl11_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sl11.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
            }
            else
            {
                _data[kanaal2 + 4] = Convert.ToByte(sl5.Value);
                sl11.Value = sl5.Value;
            }
            switch (sl11.Value)
            {
                case 0:
                    kleur1.Content = "wit";
                    next1.Content = "next: rd";
                    break;
                case 10:
                    kleur1.Content = "rood";
                    next1.Content = "next: or";
                    break;
                case 20:
                    kleur1.Content = "oranje";
                    next1.Content = "next: lgr";
                    break;
                case 30:
                    kleur1.Content = "l groen";
                    next1.Content = "next: dgr";
                    break;
                case 40:
                    kleur1.Content = "d groen";
                    next1.Content = "next: bl";
                    break;
                case 50:
                    kleur1.Content = "blauw";
                    next1.Content = "next: re";
                    break;
                case 60:
                    kleur1.Content = "roze";
                    next1.Content = "next: lbl";
                    break;
                case 70:
                    kleur1.Content = "l blauw";
                    next1.Content = "end";
                    break;
            }
        }
        private void btn5_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 4] = Convert.ToByte(0);
            _data[kanaal2 + 4] = Convert.ToByte(0);
            sl5.IsEnabled = false;
        }
        private void btn5_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            sl5.IsEnabled = true;
            _data[kanaal + 4] = Convert.ToByte(sl5.Value);
            _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
        }

        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private async void wtrd_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            sl11.IsEnabled = false;
            btn5.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 00;
                    _data[kanaal2 + 4] = 10;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 10;
                    _data[kanaal2 + 4] = 00;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void rdor_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            sl11.IsEnabled = false;
            btn5.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 10;
                    _data[kanaal2 + 4] = 20;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 20;
                    _data[kanaal2 + 4] = 10;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void orlgr_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 20;
                    _data[kanaal2 + 4] = 30;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 30;
                    _data[kanaal2 + 4] = 20;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void lgrdgr_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 30;
                    _data[kanaal2 + 4] = 40;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 40;
                    _data[kanaal2 + 4] = 30;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void dgrbl_Click(object sender, RoutedEventArgs e)
        {
            
            sl5.IsEnabled = false;
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 40;
                    _data[kanaal2 + 4] = 50;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 50;
                    _data[kanaal2 + 4] = 40;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void blre_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value; 
                    _data[kanaal + 4] = 50;
                    _data[kanaal2 + 4] = 60;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 60;
                    _data[kanaal2 + 4] = 50;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private async void relbl_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false;
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel();
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource();
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    double wacht = slidersp.Value;
                    _data[kanaal + 4] = 60;
                    _data[kanaal2 + 4] = 70;
                    await Task.Delay((int)(wacht * 1000));

                    if (_cts.Token.IsCancellationRequested) break;

                    _data[kanaal + 4] = 70;
                    _data[kanaal2 + 4] = 60;
                    await Task.Delay((int)(wacht * 1000));
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; }
        }
        private void geen_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            sl5.IsEnabled = true;
            btn5.IsEnabled = true;
            sl11.IsEnabled = true;
            _data[kanaal + 4] = Convert.ToByte(sl5.Value);
            _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speedlb.Content = "snelheid: " + slidersp.Value + "sec";
        }
        //GOBO---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl6_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (sl6.Value)
            {
                case 0:
                    gobo.Content = "gobo";
                    break;
                case 9:
                    gobo.Content = "flower";
                    break;
                case 18:
                    gobo.Content = "concentrische cirkel";
                    break;
                case 27:
                    gobo.Content = "driehoek";
                    break;
                case 36:
                    gobo.Content = "ster";
                    break;
                case 45:
                    gobo.Content = "propel spiraal";
                    break;
                case 64:
                    gobo.Content = "spiraal";
                    break;
                case 63:
                    gobo.Content = "gebroken cirkel";
                    break;
            }
            _data[kanaal + 5] = Convert.ToByte(sl6.Value);
            _data[kanaal2 + 5] = Convert.ToByte(sl6.Value);
        }
        //AUTO TILT---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl7_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 10] = Convert.ToByte(sl7.Value);
            _data[kanaal2 + 10] = Convert.ToByte(sl7.Value);

            if (sl7.Value == 200)
            {
                _data[kanaal + 0] = Convert.ToByte(0);
                _data[kanaal2 + 0] = Convert.ToByte(0);
                autotililbl.Content = "auto tilt aan";
                sl3.IsEnabled = true;
                if (sl9.Value == 100)
                {
                    _data[kanaal + 10] = Convert.ToByte(201);
                    _data[kanaal2 + 10] = Convert.ToByte(201);
                }
            }
            else
            {
                autotililbl.Content = "auto tilt uit";
                sl3.IsEnabled = true;
                _data[kanaal + 0] = Convert.ToByte(sl2.Value);
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);

                if (sl9.Value == 100)
                {
                    _data[kanaal + 10] = Convert.ToByte(sl9.Value);
                    _data[kanaal2 + 10] = Convert.ToByte(sl9.Value);
                }
            }
        }
        //AUTO PAN---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl9_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 10] = Convert.ToByte(sl9.Value);
            _data[kanaal2 + 10] = Convert.ToByte(sl9.Value);

            if (sl9.Value == 100)
            {
                _data[kanaal + 2] = Convert.ToByte(0);
                _data[kanaal2 + 2] = Convert.ToByte(0);
                rotatie.Content = "auto pan aan";
                sl2.IsEnabled = false;
                sl13.IsEnabled = false;

            
                if (sl7.Value == 200)
                {
                    _data[kanaal + 10] = Convert.ToByte(201);
                    _data[kanaal2 + 10] = Convert.ToByte(201);
                }
            }
            else
            {
                sl2.IsEnabled = true;
                rotatie.Content = "auto pan uit";
                _data[kanaal + 2] = Convert.ToByte(sl3.Value);
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);


                if (sl7.Value == 200)
                {
                    _data[kanaal + 10] = Convert.ToByte(sl7.Value);
                    _data[kanaal2 + 10] = Convert.ToByte(sl7.Value);
                }
            }
        }
        //SOUND-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _data[kanaal + 9] = Convert.ToByte(240);
            _data[kanaal2 + 9] = Convert.ToByte(240);
            auto.Visibility = Visibility.Hidden;
            manual.Visibility = Visibility.Visible;
            sl1.IsEnabled = false;
            sl2.IsEnabled = false;
            sl3.IsEnabled = false;
            sl4.IsEnabled = false;
            sl5.IsEnabled = false;
            sl6.IsEnabled = false;
            sl7.IsEnabled = false;
            sl8.IsEnabled = false;
            btn1.IsEnabled = false;
            btn4.IsEnabled = false;
            btn5.IsEnabled = false;
            btn8.IsEnabled = false;
            wtrd.IsEnabled = false;
            rdor.IsEnabled = false;
            orlgr.IsEnabled = false;
            lgrdgr.IsEnabled = false;
            dgrbl.IsEnabled = false;
            blre.IsEnabled = false;
            relbl.IsEnabled = false;
            geen.IsEnabled = false;
            slidersp.IsEnabled = false;
            split.IsEnabled = false;
            split1.IsEnabled = false;
            split2.IsEnabled = false;
            split3.IsEnabled = false;
        }
        private void sl8_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 8] = Convert.ToByte( sl8.Value);
            _data[kanaal2 + 8] = Convert.ToByte(sl8.Value);
            double sliderValue = sl8.Value;
            double labelValue = (255 - sliderValue) / 2.55; 
            speedlbl.Content ="speed "+ Math.Round(labelValue, 0)+"%";
        }
        private void btn8_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 8] = Convert.ToByte(0);
            _data[kanaal2 + 8] = Convert.ToByte(0);
            speedlbl.Content = "speed 100%";
        }
        private void btn8_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 8] = Convert.ToByte(sl8.Value);
            _data[kanaal2 + 8] = Convert.ToByte(sl8.Value);
            double sliderValue = sl8.Value;
            double labelValue = (255 - sliderValue) / 2.55; 
            speedlbl.Content = "speed " + Math.Round(labelValue, 0) + "%";
        }
        private void manuel(object sender, RoutedEventArgs e)
        {
            _data[kanaal + 9] = Convert.ToByte(0);
            _data[kanaal2 + 9] = Convert.ToByte(0);
            manual.Visibility = Visibility.Hidden;
            auto.Visibility = Visibility.Visible;
            sl1.IsEnabled = true;
            sl2.IsEnabled = true;
            sl3.IsEnabled = true;
            sl4.IsEnabled = true;
            sl5.IsEnabled = true;
            sl6.IsEnabled = true;
            sl7.IsEnabled = true;
            sl8.IsEnabled = true;
            btn1.IsEnabled = true;
            btn4.IsEnabled = true;
            btn5.IsEnabled = true;
            btn8.IsEnabled = true;
            wtrd.IsEnabled = true;
            rdor.IsEnabled = true;
            orlgr.IsEnabled = true;
            lgrdgr.IsEnabled = true;
            dgrbl.IsEnabled = true;
            blre.IsEnabled = true;
            relbl.IsEnabled = true;
            geen.IsEnabled = true;
            slidersp.IsEnabled = true;
            split.IsEnabled = true;
            split1.IsEnabled = true;
            split2.IsEnabled = true;
            split3.IsEnabled = true;
        }
        //MOVING HEAD 2---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            dmx2.Visibility = Visibility.Visible;
            dmxtextbox1.Visibility = Visibility.Visible;
            startk.Visibility = Visibility.Visible;
            split.Visibility = Visibility.Visible;
            split1.Visibility = Visibility.Visible;
            split2.Visibility = Visibility.Visible;
            split3.Visibility = Visibility.Visible;
            kanaalig2.Visibility = Visibility.Visible;
            var margin= kanaalig.Margin;
            kanaalig.Margin = new Thickness(margin.Left, margin.Top+26, margin.Right, margin.Bottom );
            _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
            _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
            _data[kanaal2 + 4] = Convert.ToByte(sl5.Value);
            _data[kanaal2 + 5] = Convert.ToByte(sl6.Value);
            _data[kanaal2 + 6] = Convert.ToByte(sl4.Value);
            _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);
            _data[kanaal2 + 8] = Convert.ToByte(sl8.Value);
            _data[kanaal2 + 10] = _data[kanaal + 10];
        }
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            split1.IsChecked = false;
            split2.IsChecked = false;
            split3.IsChecked = false;
            split.IsChecked = false;
            dmx2.Visibility = Visibility.Hidden;
            dmxtextbox1.Visibility = Visibility.Hidden;
            startk.Visibility = Visibility.Hidden;
            split.Visibility = Visibility.Hidden;
            split1.Visibility = Visibility.Hidden;
            split2.Visibility = Visibility.Hidden;
            split3.Visibility = Visibility.Hidden;
            kanaalig2.Visibility = Visibility.Hidden;
            sl11.Visibility = Visibility.Hidden;
            sl12.Visibility = Visibility.Hidden;
            sl13.Visibility = Visibility.Hidden;
            sl14.Visibility = Visibility.Hidden;
            dimmer2.Visibility = Visibility.Hidden;
            pan1.Visibility = Visibility.Hidden;
            tilt1.Visibility = Visibility.Hidden;
            kleur1.Visibility = Visibility.Hidden;
            next1.Visibility = Visibility.Hidden;
            var margin = kanaalig.Margin;
            kanaalig.Margin = new Thickness(margin.Left, margin.Top - 26, margin.Right, margin.Bottom);
            _data[kanaal2 + 0] = Convert.ToByte(0);
            _data[kanaal2 + 2] = Convert.ToByte(0);
            _data[kanaal2 + 3] = Convert.ToByte(0);
            _data[kanaal2 + 5] = Convert.ToByte(0);
            _data[kanaal2 + 6] = Convert.ToByte(0);
            _data[kanaal2 + 7] = Convert.ToByte(0);
            _data[kanaal2 + 8] = Convert.ToByte(0);
            _data[kanaal2 + 9] = Convert.ToByte(0);
            _data[kanaal2 + 10] = Convert.ToByte(0);
            _data[kanaal2 + 11] = Convert.ToByte(0);
        }
    }
}