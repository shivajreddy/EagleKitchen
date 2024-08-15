using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using EK24.EagleKitchenView;
using EK24.JsonModels;
using GalaSoft.MvvmLight.Command;

namespace EK24.VendorViewModels
{
    public class VendorStyleFinishViewModel
    {
        public string BrandName { get; set; }
        public string StyleName { get; set; }
        public string SpeciesName { get; set; }
        public string FinishName { get; set; }
        public string StyleFinishName { get; set; }
        public string ImagePath { get; set; }
    }

    public class VendorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<BrandStylesModel> brands;
        public ObservableCollection<BrandStylesModel> Brands
        {
            get { return brands; }
            set
            {
                if (brands != value)
                {
                    brands = value;
                    OnPropertyChanged(nameof(Brands));
                }
            }
        }

        private BrandStylesModel selectedBrand;
        public BrandStylesModel SelectedBrand
        {
            get { return selectedBrand; }
            set
            {
                if (selectedBrand != value)
                {
                    selectedBrand = value;
                    OnPropertyChanged(nameof(SelectedBrand));
                    LoadStylesForSelectedBrand();
                }
            }
        }

        private ObservableCollection<StyleModel> availableStyles;
        public ObservableCollection<StyleModel> AvailableStyles
        {
            get { return availableStyles; }
            set
            {
                if (availableStyles != value)
                {
                    availableStyles = value;
                    OnPropertyChanged(nameof(AvailableStyles));
                }
            }
        }

        private StyleModel selectedStyle;
        public StyleModel SelectedStyle
        {
            get { return selectedStyle; }
            set
            {
                if (selectedStyle != value)
                {
                    selectedStyle = value;
                    OnPropertyChanged(nameof(SelectedStyle));
                    LoadFinishesForSelectedStyle();
                }
            }
        }

        private ObservableCollection<SpeciesVariantModel> availableFinishes;
        public ObservableCollection<SpeciesVariantModel> AvailableFinishes
        {
            get { return availableFinishes; }
            set
            {
                if (availableFinishes != value)
                {
                    availableFinishes = value;
                    OnPropertyChanged(nameof(AvailableFinishes));
                }
            }
        }

        private SpeciesVariantModel selectedFinish;
        public SpeciesVariantModel SelectedFinish
        {
            get { return selectedFinish; }
            set
            {
                if (selectedFinish != value)
                {
                    selectedFinish = value;
                    OnPropertyChanged(nameof(SelectedFinish));
                }
            }
        }

        public ICommand SelectStyleCommand { get; private set; }
        public ICommand ApplyFinishCommand { get; private set; }

        // Constructor
        public VendorViewModel()
        {
            // Load Brands from UiDataService
            Brands = new ObservableCollection<BrandStylesModel>(UiDataService.BrandsWithStyles);

            // Initialize collections
            AvailableStyles = new ObservableCollection<StyleModel>();
            AvailableFinishes = new ObservableCollection<SpeciesVariantModel>();

            // Initialize commands
            SelectStyleCommand = new RelayCommand<object>(parameter => SelectStyle(parameter));
            ApplyFinishCommand = new RelayCommand<object>(parameter => ApplyFinish(parameter));
        }

        // Loads styles for the selected brand
        private void LoadStylesForSelectedBrand()
        {
            AvailableStyles.Clear();

            if (SelectedBrand != null)
            {
                foreach (var style in SelectedBrand.Styles)
                {
                    AvailableStyles.Add(style);
                }
            }
        }

        // Loads finishes for the selected style
        private void LoadFinishesForSelectedStyle()
        {
            AvailableFinishes.Clear();

            if (SelectedStyle != null)
            {
                foreach (var speciesVariant in SelectedStyle.SpeciesVariants)
                {
                    foreach (var finish in speciesVariant.Finishes)
                    {
                        AvailableFinishes.Add(speciesVariant);
                    }
                }
            }
        }

        // Method to select a style (invoked by command)
        private void SelectStyle(object parameter)
        {
            SelectedStyle = parameter as StyleModel;
            // Additional logic can be added here if needed
        }

        // Method to apply a finish (invoked by command)
        private void ApplyFinish(object parameter)
        {
            SelectedFinish = parameter as SpeciesVariantModel;
            // Additional logic can be added here if needed
        }
    }
}
