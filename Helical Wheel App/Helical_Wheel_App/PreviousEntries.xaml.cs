using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Helical_Wheel_App
{
    [XmlType("ProteinEntry")]
    public class ProteinEntry
    {
        public string ProteinName { get; set; }
        public string AminoSequence { get; set; }
        public ProteinEntry(string Name, string Sequence)
        {
            ProteinName = Name;
            AminoSequence = Sequence;
        }
        public ProteinEntry()
        {
            ProteinName = "";
            AminoSequence = "";
        }

    }
    public partial class PreviousEntries : ContentPage
    {
        private string ListID = "";
        private List<ProteinEntry> ProteinList = new List<ProteinEntry>();
        public PreviousEntries()
        {
            InitializeComponent();
            LoadEntries();
        }
        public void LoadEntries()
        {
            var button = new Button();
            button.Text = "Clear Entries";
            button.Clicked += Button_Clicked;
            var docmentPath = Path.Combine(FileStream.PathApp, FileStream.FileName);
            Stream stream = File.Open(docmentPath, FileMode.Open);
            List<ProteinEntry> proteins = new List<ProteinEntry>();
            using (var reader = new System.IO.StreamReader(stream))
            {
                var serializer = new XmlSerializer(typeof(List<ProteinEntry>), new XmlRootAttribute(FileStream.RootName));
                var xmlReader = System.Xml.XmlReader.Create(reader);
                if (serializer.CanDeserialize(xmlReader))
                    proteins = (List<ProteinEntry>)serializer.Deserialize(xmlReader);
            }
            var listView = new ListView();
            ProteinList = proteins;
            ListID = listView.Id.ToString();
            listView.Refreshing += ListView_Refreshing;
            listView.HasUnevenRows = true;
            listView.ItemsSource = proteins;
            listView.SeparatorColor = Color.Black;
            listView.ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                Label nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, new Binding("ProteinName", BindingMode.OneWay,null,null,"Protein Name: {0:N}"));

                Label AminoSequenceLabel = new Label();
                AminoSequenceLabel.SetBinding(Label.TextProperty,
                    new Binding("AminoSequence", BindingMode.OneWay,
                        null, null,"Sequence: {0:N}"));
                // Return an assembled ViewCell.
                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(0, 5),
                        Orientation = StackOrientation.Horizontal,
                        Children =
                                {
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            nameLabel,
                                            AminoSequenceLabel
                                        }

                                    }
                                }
                    }
                };
            });
            listView.ItemSelected += new EventHandler<SelectedItemChangedEventArgs>(OnSelection);
            double topPadding;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    topPadding = 20;
                    break;
                default:
                    topPadding = 0;
                    break;
            }
            this.Padding = new Thickness(10, topPadding, 10, 5);
            button.VerticalOptions = LayoutOptions.End;
            var searchBar = new SearchBar();
            searchBar.Placeholder = "Enter the name of protein";
            searchBar.TextChanged += SearchBarOnTextChanged;
            searchBar.HeightRequest = .05 * App.ScreenHeight;
            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    searchBar,
                    listView,
                    button
                }
            };
        }

        private void ListView_Refreshing(object sender, EventArgs e)
        {
            var list = (ListView)((StackLayout)Content).Children.Where(x => x.Id.ToString().Equals(ListID)).FirstOrDefault();
            list.ItemsSource = ProteinList;
            list.EndRefresh();
        }

        private void SearchBarOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var list = (ListView)((StackLayout) Content).Children.Where(x => x.Id.ToString().Equals(ListID)).FirstOrDefault();
            list.BeginRefresh();
            if (!string.IsNullOrEmpty(textChangedEventArgs.NewTextValue))
                list.ItemsSource =ProteinList.Where(
                        x => x.ProteinName.Contains(textChangedEventArgs.NewTextValue));
            list.EndRefresh();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var docmentPath = Path.Combine(FileStream.PathApp, FileStream.FileName);
            XDocument doc = new XDocument(new XElement(FileStream.RootName));
            doc.Save(docmentPath);
            LoadEntries();
        }

        void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            //Casting the item to the protein class
            var item = (ProteinEntry)e.SelectedItem;
            Application.Current.MainPage = new NavigationPage(new HomePage(item.ProteinName, item.AminoSequence));
        }
    }

}
