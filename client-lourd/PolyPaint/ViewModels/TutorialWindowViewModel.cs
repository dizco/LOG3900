using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PolyPaint.Helpers;

namespace PolyPaint.ViewModels
{
    internal class TutorialWindowViewModel : INotifyPropertyChanged
    {
        public TutorialWindowViewModel(string tutorialMode)
        {
            SetTutorialMode(tutorialMode);
            StepUri = $"/Resources/Tutorial/{_tutorialMode}{CurrentStep}.png";

            LoadNextStepCommand = new RelayCommand<object>(LoadNextStep, CanLoadNext);
            LoadPreviousStepCommand = new RelayCommand<object>(LoadPreviousStep, CanLoadPrevious);
            IgnoreTutorialCommand = new RelayCommand<Window>(IgnoreTutorial);
        }

        private const int StrokeMaxSteps = 11;
        private const int PixelMaxSteps = 6;

        private const string StrokeTutorial = "strokeStep";
        private const string PixelTutorial = "pixelStep";

        public RelayCommand<object> LoadNextStepCommand { get; set; }
        public RelayCommand<object> LoadPreviousStepCommand { get; set; }
        public RelayCommand<Window> IgnoreTutorialCommand { get; set; }

        private int _maxSteps;
        private int _currentStep = 1;

        private string _tutorialMode;
        
        public string StepIndex => $"{CurrentStep} / {_maxSteps}";
        private string _stepUri;

        public string StepUri
        {
            get => _stepUri;
            set
            {
                _stepUri = value;
                PropertyModified("StepUri");
            }
        }

        private int CurrentStep
        {
            get => _currentStep;
            set
            {
                _currentStep = value;
                PropertyModified("StepIndex");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool CanLoadPrevious(object o) => 1 < CurrentStep;
        private bool CanLoadNext(object o) => CurrentStep < _maxSteps;

        private void SetTutorialMode(string tutorialMode)
        {
            _tutorialMode = tutorialMode;

            if (tutorialMode == StrokeTutorial)
            {
                _maxSteps = StrokeMaxSteps;
            }
            else if (tutorialMode == PixelTutorial)
            {
                _maxSteps = PixelMaxSteps;
            }
        }

        private void LoadPreviousStep(object obj)
        {
            CurrentStep--;
            StepUri = $"/Resources/Tutorial/{_tutorialMode}{CurrentStep}.png";
        }

        private void LoadNextStep(object obj)
        {
            CurrentStep++;
            StepUri = $"/Resources/Tutorial/{_tutorialMode}{CurrentStep}.png";
        }

        private static void IgnoreTutorial(Window tutorial)
        {
            tutorial.Close();
        }

        /// <summary>
        ///     Called when a property of the ViewModel is changed.
        ///     An event is sent by the Viewmodel then
        ///     The event contains the name of the property modified. The event will be
        ///     catched by the View and the View will then update the component concerned
        /// </summary>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
