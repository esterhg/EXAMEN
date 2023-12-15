using Firebase.Database;
using ExamenMovil.Modelos;
using System.Collections.ObjectModel;
using Plugin.Maui.Audio;
using Firebase.Database.Query;


namespace ExamenMovil
{
    public partial class MainPage : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://crud-f278b-default-rtdb.firebaseio.com/");
        public ObservableCollection<Notas> ListaNotas { get; set; } = new ObservableCollection<Notas>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            CargaLista();
        }
        public void CargaLista()
        {
            client.Child("Notas")
                .AsObservable<Notas>()
                .Subscribe(Notas =>
                {
                    if (Notas != null && Notas.Object != null)
                    {

                        ListaNotas.Add(Notas.Object);
                    }
                });
        }
        private async void NuevaNota_Clicked(object sender, EventArgs e)
        {
            try
            {
                IAudioManager audioManager = new AudioManager(); 
                await Navigation.PushAsync(new NotasPage(audioManager));
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error al navegar: {ex.Message}");
            }
        }
    

      
        private void Editar_Tapped(object sender, EventArgs e)
        {

        }

        private async void Eliminar_Tapped(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var nota = (Notas)label.BindingContext;

         
            await client.Child("Notas").Child(nota.Id).DeleteAsync();
        }


    }

}
