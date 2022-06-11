using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaStreamer.Domain;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MediaStreamer.DataAccess.NetStandard;

namespace MediaStreamer.WebApplication.Services
{
    public class CompositionStoreService
    {
        public IServiceProvider ServiceProvider { get; }
        public CompositionStoreService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            
            OnStart();
        }

        protected async void OnStart()
        {
            FinishedLoading = false;
            DMEntitiesContext.UseSQLServer = true;
            //SetStatusText("Loading Database in progress...");
            var tsk = Task.Run(() =>
            //FileManipulator = new FileManipulator
            //(iDBAccess: 
                DBRepository = new DBRepository() { DB = new DMEntitiesContext() }//ServiceProvider.GetRequiredService<DMEntitiesContext>() })
            );

            //SetStatusText("Database has just been loaded.");
            //tsk.Wait();
            while (!tsk.IsCompleted)
            {
                Thread.Sleep(150);
            }
            DBRepository.EnsureCreated();
            FinishedLoading = true;
        }

        public bool FinishedLoading = true;
        public DBRepository DBRepository { get; private set; }
        
        public IEnumerable<Genre> GetGenres()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                //if (DBRepository == null)
                //    OnStart();
                while (!FinishedLoading)
                {
                    Thread.Sleep(450);
                    System.Diagnostics.Debug.WriteLine("GetCompositions() : waiting 450 ms");
                }
                //var debugCount = DBRepository.DB.GetGenres().Count();
                return DBRepository.DB.GetGenres().ToList();
            }
        }

        public IEnumerable<Album> GetAlbums()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                //if (DBRepository == null)
                //    OnStart();
                while (!FinishedLoading)
                {
                    Thread.Sleep(450);
                    System.Diagnostics.Debug.WriteLine("GetCompositions() : waiting 450 ms");
                }
                //var debugCount = DBRepository.DB.GetCompositions().Count();
                return DBRepository.DB.GetAlbums().ToList();
            }
        }

        public IEnumerable<Composition> GetCompositions()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                //if (DBRepository == null)
                //    OnStart();
                while (!FinishedLoading)
                {
                    Thread.Sleep(450);
                    System.Diagnostics.Debug.WriteLine("GetCompositions() : waiting 450 ms");
                }
                //var debugCount = DBRepository.DB.GetCompositions().Count();
                return DBRepository.DB.GetCompositions().ToList();
            }
        }

        public IEnumerable<Composition> GetAlbumCompositions(Guid albumID)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                //if (DBRepository == null)
                //    OnStart();
                while (!FinishedLoading)
                {
                    Thread.Sleep(450);
                    System.Diagnostics.Debug.WriteLine("GetCompositions() : waiting 450 ms");
                }
                //var debugCount = DBRepository.DB.GetCompositions().Count();
                return DBRepository.DB.GetCompositions().Where(c => c.AlbumID == albumID).ToList();
            }
        }

        public async Task AddCompositionAsync(string composition)
        {
            //if (DBRepository == null)
            //    OnStart(); 
            while (!FinishedLoading)
            {
                Thread.Sleep(450);
                System.Diagnostics.Debug.WriteLine("AddCompositionAsync() : waiting 450 ms");
            }
            Action action = () => 
            {   
                DBRepository
                .AddComposition("Sealkeen", "Astronaut (Instrumental)", "Spring and Autumn 2021 (Instrumental)"); 
            };
            await Task.Factory.StartNew(action);
        }
    }
}
