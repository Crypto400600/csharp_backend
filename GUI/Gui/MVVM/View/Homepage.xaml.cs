using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine;

namespace Gui.MVVM.View {
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : UserControl {
        public Homepage() {
            //From the App.xaml
            InitializeComponent();
        }
        
        private BaseDocument[] _documents;

        private void HideSuggestionsList() {
            Suggestions.Visibility = Visibility.Hidden;
            Suggestions.Height = 0;
            Suggestions.ItemsSource = new string[0];
        }

        private void ShowSuggestionsList() {
            Suggestions.Visibility = Visibility.Visible;
            Suggestions.Height = 200;
        }
        
        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                GenerateResults();
            }
        }

        private void SearchResults_SelectionChanged(object sender, RoutedEventArgs routedEventArgs) {
            try {
                var selectedItem = Suggestions.SelectedItem;
                if (selectedItem == null) return;
                
                SearchInput.Text = selectedItem.ToString();
                HideSuggestionsList();
                GenerateResults();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs routedEventArgs) {
            FetchSuggestions();
        }

        private void FetchSuggestions() {
            if (!string.IsNullOrWhiteSpace(SearchInput.Text)) {
                var pq = Querier.GetPastQueries(SearchInput.Text);
                if (pq.Length > 0) {
                    Suggestions.ItemsSource = Querier.GetPastQueries(SearchInput.Text);
                    Suggestions.Visibility = Visibility.Visible;
                    Suggestions.Height = 200;
                }
                else {
                    HideSuggestionsList();
                }
            }
            else {
                HideSuggestionsList();
            }
        }

        private void Button_OnSearch(object sender, RoutedEventArgs routedEventArgs) {
            GenerateResults();   
        }

        private async void GenerateResults() {
            if (!string.IsNullOrWhiteSpace(SearchInput.Text)) {
                HideSuggestionsList();
                var start = DateTime.Now;
                var querier = new Querier();
                _documents = await querier.Search(SearchInput.Text);
                var seconds = (DateTime.Now - start).TotalMilliseconds;

                SearchResults.Children.Clear();

                var endText = _documents.Length != 1 ? "s" : "";
                NumberOfResults.Text = $"{_documents.Length} Result{endText}";

                if (_documents.Length > 0) {
                    ResponseTime.Text = $"Response time: {seconds}ms";

                    foreach (var document in _documents) {
                        TextBlock tb = new TextBlock();
                        tb.Style = Resources["DownloadLinkWrapper"] as Style;

                        Hyperlink hyperlink = new Hyperlink();
                        Run run = new Run();

                        run.Text = document.Name;
                        hyperlink.NavigateUri = new Uri(document.Url);
                        hyperlink.Style = Resources["DownloadLink"] as Style;
                        hyperlink.Inlines.Add(run);

                        hyperlink.RequestNavigate += (_, e) => { System.Diagnostics.Process.Start(e.Uri.ToString()); };

                        tb.Inlines.Add(hyperlink);
                        SearchResults.Children.Add(tb);
                    }
                }
                else {
                    TextBlock tb = new TextBlock();
                    tb.Style = Resources["DownloadLinkWrapper"] as Style;
                    tb.Text = $"No Items match your search query: {SearchInput.Text}";
                    SearchResults.Children.Add(tb);
                }

                Suggestions.ItemsSource = Querier.GetPastQueries(SearchInput.Text);
            }
        }
    }
}