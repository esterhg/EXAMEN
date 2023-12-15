using ExamenMovil.Modelos;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace ExamenMovil
{
    public partial class NotasPage : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://crud-f278b-default-rtdb.firebaseio.com/");
        FirebaseStorage firebaseStorage = new FirebaseStorage("crud-f278b.appspot.com");
        private static int lastUsedId = 1;
        private string UrlFoto { get; set; }
        private string UrlAudio { get; set; }
        readonly IAudioManager _audioManager ;
        readonly IAudioRecorder _audioRecorder;

        public NotasPage(IAudioManager audioManager)
        {
            InitializeComponent();
            BindingContext = this;
            fechaPicker.Date = DateTime.Now;
            _audioManager = audioManager;
            _audioRecorder= audioManager.CreateRecorder();
            imgFoto.Source = ImageSource.FromFile("ruta_de_tu_imagen_inicial");
        }

        private async void TomarFoto_Clicked(object sender, EventArgs e)
        {
            var foto = await MediaPicker.CapturePhotoAsync();

            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                UrlFoto = await new FirebaseStorage("basegrupo2.appspot.com")
                    .Child("Fotos")
                    .Child(DateTime.Now.ToString("ddMMyyyyhhmmss") + "_captura.jpg")
                    .PutAsync(stream);
                imgFoto.Source = UrlFoto;
            }
        }


        private async void Grabar_Clicked(object sender, EventArgs e)
        {
            if (await Permissions.RequestAsync<Permissions.Microphone>() != PermissionStatus.Granted)
            {
                return;
            }

            if (_audioRecorder != null && !_audioRecorder.IsRecording)
            {
                await _audioRecorder.StartAsync();
            }
            else if (_audioRecorder != null)
            {
                var recordeAudio = await _audioRecorder.StopAsync();

                if (recordeAudio != null)
                {
                    var player = AudioManager.Current.CreatePlayer(recordeAudio.GetAudioStream());
                    player.Play();

                    UrlAudio = await SubirArchivoAsync("audios", "nombre_audio.mp3", recordeAudio.GetAudioStream());
                }
            }
        }


        private async void Guardar_Clicked(object sender, EventArgs e)
        {
            if ( string.IsNullOrEmpty(Titulo.Text) || string.IsNullOrEmpty(Descripcion.Text))
            {
                
                await DisplayAlert("Error", "Por favor, complete todos los campos y adjunte una foto y grabación de audio.", "OK");
                return;
            }

            int newId = GetNextId();
            Notas nuevaNota = new Notas
                {
  
                    Titulo = Titulo.Text,
                    Descripcion = Descripcion.Text,
                    Fecha = fechaPicker.Date,
                    Urlfoto = UrlFoto,
                    UrlAudio = UrlAudio
                };

                await firebaseClient.Child("Notas").Child(newId.ToString()).PutAsync(nuevaNota);
               
        }

        private int GetNextId()
        {
            
            return ++lastUsedId;
        }
        private async Task<string> SubirArchivoAsync(string directorio, string nombreArchivo, Stream stream)
        {
            try
            {
                var referencia = await firebaseStorage.Child(directorio).Child(nombreArchivo).PutAsync(stream);
                var downloadUrl = await firebaseStorage.Child(directorio).Child(nombreArchivo).GetDownloadUrlAsync();
                return downloadUrl.ToString();
            }
            catch (FirebaseStorageException ex)
            {
                Console.WriteLine($"Error al subir archivo al Firebase Storage: {ex.Message}");
                return null;
            }
        }


    }
}
