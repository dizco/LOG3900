using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using PolyPaint.Vues;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace PolyPaint.VueModeles
{

    /// <summary>
    /// Sert d'intermédiaire entre la vue et le modèle.
    /// Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    /// Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    class VueModele : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();

        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }            
            set { ProprieteModifiee(); }
        }        
        
        public string CouleurSelectionnee
        {
            get { return editeur.CouleurSelectionnee; }
            set { editeur.CouleurSelectionnee = value; }
        }

        public string PointeSelectionnee
        {
            get { return editeur.PointeSelectionnee; }
            set { ProprieteModifiee(); }
        }

        public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }
       
        public StrokeCollection Traits { get; set; }

        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<object> Empiler { get; set; }
        public RelayCommand<object> Depiler { get; set; }
        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }
        public RelayCommand<object> Reinitialiser { get; set; }   
        
        //Command for managing the views
        public RelayCommand<object> ShowLoginWindowCommand { get; set; }
        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        //Command for sending editor actions to server
        public RelayCommand<Stroke> SendNewStrokeCommand { get; set; }

        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        public VueModele()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);
            editeur.EditorAddedStroke += OnStrokeCollectedHandler;

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            AttributsDessin = new DrawingAttributes();            
            AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            AjusterPointe();

            Traits = editeur.traits;
            
            // Pour chaque commande, on effectue la liaison avec des méthodes du modèle.            
            Empiler = new RelayCommand<object>(editeur.Empiler, editeur.PeutEmpiler);            
            Depiler = new RelayCommand<object>(editeur.Depiler, editeur.PeutDepiler);
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChoisirPointe = new RelayCommand<string>(editeur.ChoisirPointe);
            ChoisirOutil = new RelayCommand<string>(editeur.ChoisirOutil);
            Reinitialiser = new RelayCommand<object>(editeur.Reinitialiser); 
            
            //Outgoing editor actions
            SendNewStrokeCommand = new RelayCommand<Stroke>(SendNewStroke);

            //Managing different View
            ShowLoginWindowCommand = new RelayCommand<object>(ShowLoginWindow);
        }

        /// <summary>
        /// Appelee lorsqu'une propriété de VueModele est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        /// alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Traite les évènements de modifications de propriétés qui ont été lancés à partir
        /// du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">Les paramètres de l'évènement. PropertyName est celui qui nous intéresse. 
        /// Il indique quelle propriété a été modifiée dans le modèle.</param>
        private void EditeurProprieteModifiee(object sender, PropertyChangedEventArgs e)
        {     
            if (e.PropertyName == "CouleurSelectionnee")
            {
                AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            }                
            else if (e.PropertyName == "OutilSelectionne")
            {
                OutilSelectionne = editeur.OutilSelectionne;
            }                
            else if (e.PropertyName == "PointeSelectionnee")
            {
                PointeSelectionnee = editeur.PointeSelectionnee;
                AjusterPointe();
            }
            else // e.PropertyName == "TailleTrait"
            {               
                AjusterPointe();
            }                
        }

        /// <summary>
        /// C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (TailleTrait).
        /// Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe 
        /// horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
        private void AjusterPointe()
        {
            AttributsDessin.StylusTip = (editeur.PointeSelectionnee == "ronde") ? StylusTip.Ellipse : StylusTip.Rectangle;
            AttributsDessin.Width = (editeur.PointeSelectionnee == "verticale") ? 1 : editeur.TailleTrait;
            AttributsDessin.Height = (editeur.PointeSelectionnee == "horizontale") ? 1 : editeur.TailleTrait;
        }

       
        //Show login window
        public void ShowLoginWindow(object o)
        {
            if ( loginWindow == null)
            {
                loginWindow = new Vues.LoginWindowView();
                loginWindow.Show();
                loginWindow.Closed += new EventHandler(AddItemView_Closed);
            }

            else
            {
                loginWindow.Activate();
            }
        }

        private void AddItemView_Closed(object sender, EventArgs e)
        {
            loginWindow = null;
        }


        /// <summary>
        ///     Handler for InkCanvas events
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            SendNewStrokeCommand.Execute(e.Stroke);
        }

        /// <summary>
        ///     Handler for Editeur events (depilage)
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, Stroke stroke)
        {
            SendNewStrokeCommand.Execute(stroke);
        }

        private void SendNewStroke(Stroke stroke)
        {
            Messenger?.SendEditorActionNewStroke(stroke);
        }

    }
}