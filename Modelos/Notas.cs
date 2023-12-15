using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Firebase.Database;
using Firebase.Storage;
using Microsoft.Maui.Storage;
using Firebase.Storage;

namespace ExamenMovil.Modelos
{
    public class Notas
    {
        FirebaseClient client = new FirebaseClient("https://crud-f278b-default-rtdb.firebaseio.com/");
        public async Task<int> GetCounterAsync()
        {
            var counterSnapshot = await client.Child("contador").OnceSingleAsync<int?>();
            return counterSnapshot ?? 0;
        }

        public async Task UpdateCounterAsync(int newCounterValue)
        {
            await client.Child("contador").PutAsync(newCounterValue);
        }

        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Urlfoto { get; set; }
        public string UrlAudio { get; set; }
    }
}
