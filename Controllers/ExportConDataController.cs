using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using MyFriendsApp.Data;

namespace MyFriendsApp.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/friends/csv")]
        [HttpGet("/export/ConData/friends/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFriends(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/friends/excel")]
        [HttpGet("/export/ConData/friends/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFriends(), Request.Query), fileName);
        }
    }
}
