using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using PolyPaint.Helpers;

namespace PolyPaint.ViewModels
{
    internal class TutorialWindowViewModel : INotifyPropertyChanged
    {
        public TutorialWindowViewModel()
        {
            LoadNextStepCommand = new RelayCommand<object>(LoadNextStep);
            LoadPreviousStepCommand = new RelayCommand<object>(LoadPreviousStep);
            IgnoreTutorialCommand = new RelayCommand<Window>(IgnoreTutorial);
        }

        private const int MaxSteps = 5;

        public RelayCommand<object> LoadNextStepCommand { get; set; }
        public RelayCommand<object> LoadPreviousStepCommand { get; set; }
        public RelayCommand<Window> IgnoreTutorialCommand { get; set; }

        private int _currentStep = 1;

        public string StepIndex => $"{CurrentStep} / {MaxSteps}";
        private string _stepUri = "/Resources/Tutorial/strokeStep1.png";
        
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

        private bool CanLoadPrevious() => 1 < CurrentStep;
        private bool CanLoadNext() => CurrentStep < MaxSteps;

        private void LoadPreviousStep(object obj)
        {
            if (CanLoadPrevious())
            {
                CurrentStep--;
                StepUri = $"/Resources/Tutorial/strokeStep{CurrentStep}.png";
            }
            
        }

        private void LoadNextStep(object obj)
        {
            if (CanLoadNext())
            {
                CurrentStep++;
                StepUri = $"/Resources/Tutorial/strokeStep{CurrentStep}.png";
            }
        }

        private void IgnoreTutorial(Window tutorial)
        {
            tutorial.Close();
        }
    }
}
