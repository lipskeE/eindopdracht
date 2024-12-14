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
using eindopdracht_V2;


namespace eind_opdracht
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        textd oManuelclass = new textd();
        byte[] _data;
        DispatcherTimer _dispatcherTimer;
        const int NUMBER_OF_DMX_BYTES = 513;
        int kanaal;
        int kanaal2;

        public MainWindow()
        {
            InitializeComponent();
            _data = new byte[NUMBER_OF_DMX_BYTES];
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(0.025);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();

            _cts = new CancellationTokenSource(); 
            _serialPort = new SerialPort();
            manual.Visibility = Visibility.Hidden;
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
        private void Button_Click_2(object sender, RoutedEventArgs e) //Zorgt er voor dat de poort opnieuw kan geladen worden zonder het programma te sluiten
        {
            Port.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
                Port.Items.Add(s);

            _serialPort = new SerialPort();
            _serialPort.BaudRate = 250000;
            _serialPort.StopBits = StopBits.Two;
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

                if (Port.SelectedItem !=null)
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
            e.Handled = !IsTextNumeric(e.Text); //zorgt er voor dat je alleen nummers kan invoeren
        }
        private bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$"); //zorgt er voor dat je alleen nummers kan invoeren
        }
        private void dmxtextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(dmxtextbox.Text, out int value)) //waneer er een getal groter dan 512 wordt ingetypt komt er een fout.
            {
                if (value > 512)
                {
                    MessageBox.Show("DMX gaat tot 512", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dmxtextbox.Text = "512";
                    dmxtextbox.SelectionStart = dmxtextbox.Text.Length;
                }
            }
            else if (!string.IsNullOrEmpty(dmxtextbox.Text)) //zet de textbox terug op leeg
            {
                dmxtextbox.Text = "";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) //waneer er een moving head wordt gebruikt dan zal hij de value's pakken van de sliders die al waren ingesteld
        {
            if (int.TryParse(dmxtextbox.Text, out int newAddress))
                kanaal = newAddress;
            kanaalig.Content = "kanaal in gebruik: " + kanaal;
            sl8.Value = 253;
            _data[kanaal + 0] = Convert.ToByte(sl2.Value);
            _data[kanaal + 2] = Convert.ToByte(sl3.Value);
            _data[kanaal + 4] = Convert.ToByte(sl5.Value);
            _data[kanaal + 5] = Convert.ToByte(sl6.Value);
            _data[kanaal + 6] = Convert.ToByte(sl4.Value);
            _data[kanaal + 7] = Convert.ToByte(sl1.Value);
            _data[kanaal + 8] = Convert.ToByte(sl8.Value);
            _data[kanaal + 9] = Convert.ToByte(sl9.Value);
        }
        private void dmxtextbox_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric1(e.Text); //zorgt er voor dat je alleen nummers kan invoeren
        }
        private bool IsTextNumeric1(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$"); //zorgt er voor dat je alleen nummers kan invoeren
        }
        private void dmxtextbox_TextChanged1(object sender, TextChangedEventArgs e) //heeft een waarschuwing dat de textbox max tot 512 kan dit is voor de tweede textbox.
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
            else if (!string.IsNullOrEmpty(dmxtextbox1.Text)) //zet de textbox terug op leeg
            {
                dmxtextbox1.Text = "";
            }
        }
        private void Button2(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(dmxtextbox1.Text, out int newAddress))
                kanaal2 = newAddress;
            kanaalig2.Content = "kanaal2 in gebruik: " + kanaal2;
            _data[kanaal2 + 0] = _data[kanaal + 0]; //zorgt er voor als er een tweede moving head wordt aangesloten 
            _data[kanaal2 + 2] = _data[kanaal + 2]; // dat deze de eerste zal volgen.
            _data[kanaal2 + 4] = _data[kanaal + 4];
            _data[kanaal2 + 5] = _data[kanaal + 5];
            _data[kanaal2 + 6] = _data[kanaal + 6];
            _data[kanaal2 + 7] = _data[kanaal + 7];
            _data[kanaal2 + 8] = _data[kanaal + 8];
            _data[kanaal2 + 9] = _data[kanaal + 9];
            _data[kanaal2 +10] = _data[kanaal + 10];
        }
        //DIMMEN---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dimmer.Content = "dim=" + Math.Round(sl1.Value / 2.55, 0) + "%"; //sl1 value omzetten naar leesbare percentage.
            _data[kanaal + 7] = Convert.ToByte(sl1.Value);
            _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);

            if (sl12.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl12.Value); //zorgt er voor als split wordt gebruikt dat de juiste lamp de juiste slider volgt.
            }
            else
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl1.Value); //zorgt waneeer split wordt uitgeschakeld dat hij terug de eerst slider volgt en dat hij sl12 in de achtergrond aanpast
                sl12.Value=sl1.Value;
            }
        }
        private void btn1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 7] = Convert.ToByte(255); //wanneer full wordt ingeduwt gaat kanaal+7 en kaanaal2+7 op 255 staan. Dus full.
            _data[kanaal2 + 7] = Convert.ToByte(255); 
            dimmer.Content = oManuelclass.Text;
            dimmer2.Content= oManuelclass.Text;
        }
        private void btn1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _data[kanaal + 7] = Convert.ToByte(sl1.Value); //wanneer full wordt losgelaten dat de value van kaanaal+7 en kanaal2+7 terug op hun sliders staan
            _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
            dimmer.Content = "dim=" + Math.Round(sl1.Value / 2.55, 0) + "%"; 
            dimmer2.Content = "dim=" + Math.Round(sl12.Value / 2.55, 0) + "%";
        }
        private void split1_Checked(object sender, RoutedEventArgs e)
        {
            if (sl1 != null)
            {
                var margin = sl1.Margin; //zorgt er voor dat erplaats is voor een tweede slideren toont de tweede slider.
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
                var margin = sl1.Margin;  //zet alles terug naar normaal. En haalt de tweede slider weg 
                var margin1= dimmer.Margin;
                sl1.Margin = new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                dimmer.Margin = new Thickness(margin1.Left + 17.5, margin1.Top, margin.Right + 17.5, margin.Bottom);
                sl12.Visibility = Visibility.Hidden;
                dimmer2 .Visibility = Visibility.Hidden;
                _data[kanaal2 + 7] = Convert.ToByte(sl1.Value);
                sl12.Value = sl1.Value;
            }
        }
        private void sl12_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dimmer2.Content = "dim2=" + Math.Round(sl12.Value / 2.55, 0) + "%"; //sl12 value omzetten naar leesbare percentage.

            if (sl12.Visibility == Visibility.Visible) //zorgt er voor als split wordt gebruikt dat de juiste lamp de juiste slider volgt.
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl12.Value);
            }
            else
            {
                _data[kanaal2 + 7] = Convert.ToByte(sl1.Value); //zorgt waneeer split wordt uitgeschakeld dat hij terug de eerst slider volgt en dat hij sl12 in de achtergrond aanpast
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
                var margin = sl2.Margin; //zorgt er voor dat de slider wordt verplaatst om plaats te maken voor de tweede slider
                var margin2 = pan.Margin;
                sl2.Margin = new Thickness(margin.Left - 17.5, margin.Top, margin.Right - 17.5, margin.Bottom);
                pan.Margin = new Thickness(margin2.Left - 17.5, margin2.Top, margin2.Right - 17.5, margin2.Bottom);
            }
            _data[kanaal2 + 0] = Convert.ToByte(sl13.Value);
        }
        private void split2_Unchecked(object sender, RoutedEventArgs e)
        {
            sl13.Visibility = Visibility.Hidden; //zorgt dat alles terug gaat naar één slider
            pan1.Visibility = Visibility.Hidden;

            if (sl2 != null)
            {
                var margin = sl2.Margin; //zorgt er voor dat alles terug gaat naar hun origineel
                var margin2 = pan.Margin;
                sl2.Margin = new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                pan.Margin = new Thickness(margin2.Left + 17.5, margin2.Top, margin2.Right + 17.5, margin2.Bottom);
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
                sl13.Value = sl2.Value;
            }
        }
        private void sl13_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pan1.Content = "pan2 " + Math.Round(sl13.Value * 2.1176470, 0) + "°"; //zet de value van sl13 om in leesbare tekst

            if (sl13.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl13.Value); //zet de value vans sl13 om in byte
            }
            else
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value); //zorgt er voor als sl13 niet zichtbaar is dat kanaal2 +0 sl2 volgt
                
            }
        }
        private void sl2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 0] = Convert.ToByte(sl2.Value);
            _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
            pan.Content = "pan " + Math.Round(sl2.Value * 2.1176470, 0) + "°"; //zet de value van sl2 om in leesbare tekst

            if (sl13.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl13.Value); //als SL13 visible is dan is kanaal2 +0 =sl13.value
            }
            else
            {
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value); //als SL13 niet visible is dan is kanaal2 +0 =sl2.value
                sl13.Value=sl2.Value;
            }
        }
        
        //TITLT---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _data[kanaal + 2] = Convert.ToByte(sl3.Value);
            _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
            tilt.Content = "tilt " + Math.Round(sl3.Value * 0.8039215, 0) + "°"; //zet de value van sl3 om in leesbare tekst
            if (sl14.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);
            }
            else
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
                sl14.Value=sl3.Value;
            }
        }
        private void split3_Checked(object sender, RoutedEventArgs e)
        {

            sl14.Visibility = Visibility.Visible;
            tilt1.Visibility = Visibility.Visible;

            if (sl3 != null) //zorgt er voor dat de slider wordt verplaatst om plaats te maken voor de tweede slider
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
                var margin = sl3.Margin; //zorgt er voor dat alles terug gaat naar hun origineel
                var margin1= tilt.Margin;
                sl3.Margin= new Thickness(margin.Left + 17.5, margin.Top, margin.Right + 17.5, margin.Bottom);
                tilt.Margin = new Thickness(margin1.Left + 17.5, margin1.Top, margin1.Right + 17.5, margin1.Bottom);
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
                sl14.Value = sl3.Value;
            }
        }
        private void sl14_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tilt1.Content = "tilt2 " + Math.Round(sl14.Value * 0.8039215, 0) + "°"; //zet de value van sl4 om in leesbare tekst

            if (sl14.Visibility == Visibility.Visible)
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);
            }
            else
            {
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);
                sl14.Value=sl3.Value;
            }
        }
        //STROBO---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strobo.Content = "strobo=" + sl4.Value / 2 + "%"; //zet de value van sl4 om in leesbare tekst
            _data[kanaal + 6] = Convert.ToByte(sl4.Value);
            _data[kanaal2 + 6] = Convert.ToByte(sl4.Value);
        }
        private void btn4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) //alleen als slider 4=0 dan kan de full strobo maar werken
        {
            if (sl4.Value == 0)
            {
                _data[kanaal + 6] = Convert.ToByte(200);
                _data[kanaal2 + 6] = Convert.ToByte(200);
                strobo.Content = "strobo=100%";
            }
        }
        private void btn4_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //zet de strobo terug naar 0
        {
            if (_data[kanaal + 6] == 200)
            {
                _data[kanaal + 6] = Convert.ToByte(0);
                _data[kanaal2 + 6] = Convert.ToByte(0);
                strobo.Content = "strobo=0%";
            }
        }
        //KLEUR---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl5_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //zet de value van slider 5 om in bytes voor kanaal +4
        {
            switch (sl5.Value)
            {
                case 0:
                    kleur.Content = "wit"; //zet de value van sl5 om in leesbare tekst
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
            }
        }
        private void split_Checked(object sender, RoutedEventArgs e) //zorgt er voor dat de slider wordt verplaatst om plaats te maken voor de tweede slider
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
                sl11.Value = sl5.Value;
            }
            _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
        }
        private void split_Unchecked(object sender, RoutedEventArgs e) //zorgt er voor dat de slider terug naar hun originele positie worden gezet.
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
                sl11.Value = sl5.Value;
                _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
            }
        }
        private void sl11_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //als slider 11 visible is gebruikt kanaal2+4 slider 11 anders gebruikt hij slider 5
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
                    kleur1.Content = "wit"; //zet de value van sl11 om in leesbare tekst
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
        private void btn5_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) //zet alle lampen op wit zo lang de knop is ingeduwt
        {
            _data[kanaal + 4] = Convert.ToByte(0);
            _data[kanaal2 + 4] = Convert.ToByte(0);
            sl5.IsEnabled = false;
            sl11.IsEnabled = false;
            kleur.Content = "wit";
            kleur1.Content = "wit";
        }
        
        private void btn5_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //zet alle lampen terug naar de juiste sliders
        {
           sl5.IsEnabled = true;
           sl11.IsEnabled = true;
           _data[kanaal + 4] = Convert.ToByte(sl5.Value);
           _data[kanaal2 + 4] = Convert.ToByte(sl11.Value);
        }

        private CancellationTokenSource _cts; // Wordt gebruikt om asynchrone operaties te annuleren
        private bool _isRunning = false;     // Houdt bij of een proces momenteel actief is
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void wtrd_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            sl11.IsEnabled = false;
            btn5.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return;  // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wisselt tussen wit en rood
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 00; //zet kanaal +4 op wit
                    _data[kanaal2 + 4] = 10; //zet kaanaal2 +4 op rood
                    wtrd.Background=new SolidColorBrush(Colors.White); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 10; //zet kanaal +4 op rood
                    _data[kanaal2 + 4] = 00; //zet kanaal2 +4 op wit
                    wtrd.Background = new SolidColorBrush(Colors.Red); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            wtrd.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void rdor_Click(object sender, RoutedEventArgs e) 
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            sl11.IsEnabled = false;
            btn5.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; 
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wisselt tussen rood en oranje
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 10; //zet kanaal2 +4 op rood
                    _data[kanaal2 + 4] = 20; //zet kanaal2 +4 op oranje
                    rdor.Background = new SolidColorBrush(Colors.Red); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 20; //zet kanaal +4 op oranje
                    _data[kanaal2 + 4] = 10; //zet kanaal2 +4 op rood
                    rdor.Background= new SolidColorBrush(Colors.Orange); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            rdor.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void orlgr_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wisselt tussen oranjeen lichtgroen
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 20; //zet kanaal +4 op oranje
                    _data[kanaal2 + 4] = 30; //zet kanaal2 +4 op lichtgroen
                    orlgr.Background=new SolidColorBrush(Colors.Orange); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 30; //zet kanaal +4 op lichtgroen
                    _data[kanaal2 + 4] = 20; //zet kanaal2 +4 op oranje
                    orlgr.Background=new SolidColorBrush(Colors.LightGreen); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            orlgr.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void lgrdgr_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wislet tussen lichtgroen en donkergroen
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 30; //zet kanaal +4 op lichtgroen
                    _data[kanaal2 + 4] = 40; //zet kanaal2 +4 op donkergroen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                    lgrdgr.Background=new SolidColorBrush(Colors.LightGreen); // Achtergrondkleur aanpassen

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 40; //zet kanaal +4 op donkergroen
                    _data[kanaal2 + 4] = 30; //zet kanaal2 +4 op lichtgroen
                    lgrdgr.Background=new SolidColorBrush(Colors.DarkGreen); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            lgrdgr.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void dgrbl_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wiselt tussen donker groen en blauw.
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 40; //zet kanaal +4 op donkergroen
                    _data[kanaal2 + 4] = 50; //zet kanaal2 +4 op blauw
                    dgrbl.Background=new SolidColorBrush(Colors.DarkGreen); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 50; //zet kanaal +4 op blauw
                    _data[kanaal2 + 4] = 40; //zet kanaal2 +4 op donkergroen
                    dgrbl.Background = new SolidColorBrush(Colors.Blue); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            dgrbl.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void blre_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wiselt tussen blauw en roze
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 50; //zet kanaal +4 op blauw
                    _data[kanaal2 + 4] = 60; //zet kanaal2 +4 op roze
                    blre.Background=new SolidColorBrush(Colors.Blue); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 60; //zet kanaal +4 op roze
                    _data[kanaal2 + 4] = 50; //zet kanaal2 +4 op blauw
                    blre.Background = new SolidColorBrush(Colors.Pink); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            blre.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void relbl_Click(object sender, RoutedEventArgs e)
        {
            sl5.IsEnabled = false; // Schakel sliders en knoppen uit om manuele aanpassingen te voorkomen
            btn5.IsEnabled = false;
            sl11.IsEnabled = false;
            _cts?.Cancel(); // Annuleer een bestaande taak, indien actief
            if (_isRunning) return; // Zorg ervoor dat geen meerdere taken tegelijk draaien
            _isRunning = true;
            _cts = new CancellationTokenSource(); // Nieuw annuleertoken maken

            try
            {
                while (!_cts.Token.IsCancellationRequested) //wiselt tussen roze en licht blauw.
                {
                    double wacht = slidersp.Value; // Wachtperiode in seconden ophalen van de slider
                    _data[kanaal + 4] = 60; //zet kanaal +4 op roze
                    _data[kanaal2 + 4] = 70; //zet kanaal2 +4 op lichtblauw
                    relbl.Background= new SolidColorBrush(Colors.Pink); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000

                    if (_cts.Token.IsCancellationRequested) break; // Controleer op annulering

                    _data[kanaal + 4] = 70; //zet kanaal +4 op lichtblauw
                    _data[kanaal2 + 4] = 60; //zet kanaal2 +4 op roze
                    relbl.Background=new SolidColorBrush(Colors.LightBlue); // Achtergrondkleur aanpassen
                    await Task.Delay((int)(wacht * 1000)); //wacht maal 1000
                }
            }
            catch (OperationCanceledException) { }
            finally { _isRunning = false; } // Zet running status naar false na voltooiing
            relbl.Background = new SolidColorBrush(Colors.Gray); // Achtergrondkleur aanpassen naar grijs
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void geen_Click(object sender, RoutedEventArgs e) //stopt alle automatische kleuren wisseling.
        {
            _cts?.Cancel(); // canecelt de cts token en enabeled terug de sliders. en zet deze sliders terug op hun bestaande value's
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
        private void sl6_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //zet de value van sl6 om in bytes vooor kanaal +5
        {
            switch (sl6.Value) //zet sl6.value om in leesbare tekst.
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
                case 54:
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
        private void sl7_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // zet auto tilt aan
        {
            _data[kanaal + 10] = Convert.ToByte(sl7.Value); 
            _data[kanaal2 + 10] = Convert.ToByte(sl7.Value);

            if (sl7.Value == 200)
            {
               
                autotililbl.Content = "auto tilt aan";
                sl3.IsEnabled = true;
                if (sl9.Value == 100)
                {
                    _data[kanaal + 10] = Convert.ToByte(201); //als slider9.value=100 dan zet hij kanaal +10 op 201. Dit zorgt ervoor dat hij automatische tlit en pan gebrukt
                    _data[kanaal2 + 10] = Convert.ToByte(201);
                }
            }
            else
            {
                autotililbl.Content = "auto tilt uit"; //als auto tilt uit wordt gedaan. dan zal sl3 terug enabeled worden. en kanaal +0 gaa terug naar de value's van de sliders
                sl3.IsEnabled = true;
                _data[kanaal + 0] = Convert.ToByte(sl2.Value);
                _data[kanaal2 + 0] = Convert.ToByte(sl2.Value);
            }
        }

        //AUTO PAN---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void sl9_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //zet de value van slider 9 om in bytes voor kanaal +10
        {
            _data[kanaal + 10] = Convert.ToByte(sl9.Value);
            _data[kanaal2 + 10] = Convert.ToByte(sl9.Value);

            if (sl9.Value == 100) //als sl9.value=100 dan gaat hij automatishe pan doen
            {
                sl2.IsEnabled = false;
                sl13.IsEnabled = false;
                _data[kanaal + 0] = Convert.ToByte(0);
                _data[kanaal2 + 0] = Convert.ToByte(0);
                rotatie.Content = "auto pan aan";
                _data[kanaal + 2] = Convert.ToByte(sl3.Value);
                _data[kanaal2 + 2] = Convert.ToByte(sl14.Value);

                if (sl7.Value == 200) //als slider7.value=200 dan zet hij kanaal +10 op 201. Dit zorgt ervoor dat hij automatische tlit en pan gebrukt
                {
                    _data[kanaal + 10] = Convert.ToByte(201);
                    _data[kanaal2 + 10] = Convert.ToByte(201);
                }
            }
            else //als auto pan uit wordt gedaan. dan zal sl3 terug enabeled worden. en kanaal +2 gaa terug naar de value's van de sliders
            {
                sl2.IsEnabled = true;
                sl13.IsEnabled = true;
                rotatie.Content = "auto pan uit";
                _data[kanaal + 2] = Convert.ToByte(sl3.Value);
                _data[kanaal2 + 2] = Convert.ToByte(sl3.Value);


                if (sl7.Value == 200) //als slider7.value=200 dan zet hij kanaal +10 terug op de warrden van de juiste slider
                {
                    _data[kanaal + 10] = Convert.ToByte(sl7.Value);
                    _data[kanaal2 + 10] = Convert.ToByte(sl7.Value);
                }
            }
            if (sl9.Value == 0) //als slider9.value=0 dan zet hij kanaal +0 terug op de warrden van de juiste slider
            {
                _data[kanaal + 0] = Convert.ToByte(sl2.Value);
                _data[kanaal2 + 0] = Convert.ToByte(sl13.Value);
            }
        }
        //SOUND-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Button_Click_1(object sender, RoutedEventArgs e) //zet alle sliders en buttens uit en schakelt de microfonen aan in de moving heads.
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
        private void sl8_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //zet de value van slider 8 om naar bytes voor kanaal9
        {
            _data[kanaal + 8] = Convert.ToByte( sl8.Value);
            _data[kanaal2 + 8] = Convert.ToByte(sl8.Value);
            double sliderValue = sl8.Value;
            double labelValue = (255 - sliderValue) / 2.55; 
            speedlbl.Content ="speed "+ Math.Round(labelValue, 0)+"%"; //zet sl8.value om in leesbare tekst
        }
        private void btn8_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) //zet speed op max
        {
            _data[kanaal + 8] = Convert.ToByte(0); 
            _data[kanaal2 + 8] = Convert.ToByte(0);
            speedlbl.Content = "speed 100%";
        }
        private void btn8_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //zet speed terug op de waarde van de sliders.
        {
            _data[kanaal + 8] = Convert.ToByte(sl8.Value);
            _data[kanaal2 + 8] = Convert.ToByte(sl8.Value);
            double sliderValue = sl8.Value;
            double labelValue = (255 - sliderValue) / 2.55; 
            speedlbl.Content = "speed " + Math.Round(labelValue, 0) + "%";
        }
        private void manuel(object sender, RoutedEventArgs e)//zet alle sliders en buttens aa en schakelt de microfonen uit in de moving heads.
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
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e) //zorgt er voor als moving head 2 word ingeschakeld dat hij de waardes gebruik van de eerste slider.
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
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e) //zorgt er voor als moving head 2 wordt uitgeschakeld dat hij de waardes gebruik  van de eerste slider.
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
            _data[kanaal2 + 4] = Convert.ToByte(0);
            _data[kanaal2 + 5] = Convert.ToByte(0);
            _data[kanaal2 + 6] = Convert.ToByte(0);
            _data[kanaal2 + 7] = Convert.ToByte(0);
            _data[kanaal2 + 8] = Convert.ToByte(0);
            _data[kanaal2 + 9] = Convert.ToByte(0);
            _data[kanaal2 + 10] = Convert.ToByte(0);
        }
    }
}