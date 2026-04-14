using ProiectII.Data;
using ProiectII.Models;
using ProiectII.Interfaces;

namespace ProiectII.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {


        public LocationRepository(ApplicationDbContext context) : base(context) { }


        /// de implementat metode specifice pentru Location, daca e nevoie. Daca nu, putem lasa doar cele mostenite din GenericRepository.





    }
}
