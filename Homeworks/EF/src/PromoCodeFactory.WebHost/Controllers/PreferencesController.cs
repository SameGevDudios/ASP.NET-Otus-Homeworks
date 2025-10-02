using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : Controller
    {
        private IRepository<Preference> _preferenceRepository;

        public PreferencesController(IRepository<Preference> preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Список всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<PreferenceResponse>> GetAllPreferencesAsync()
        {
            var preferences = await _preferenceRepository.GetAllAsync();
            var preferenceResponseList = preferences.Select(x =>
            new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            return preferenceResponseList;
        }
    }
}
