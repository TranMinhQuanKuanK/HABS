using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.Interfaces.User;

namespace HASB_User
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int executionCount = 0;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        private readonly IPatientService _patientService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var count = Interlocked.Increment(ref executionCount);

                /*PatientCreateEditModel patient = new PatientCreateEditModel
                {
                    PhoneNumber = "0337746758",
                    Name = "Tên Bệnh Nhân " + count.ToString(),
                    Gender = 0,
                    DateOfBirth = DateTime.Now,
                    Address = "Đồng Tháp",
                    Bhyt = "HABS-123456"
                };

                var url = await _patientService.RegisterANewPatient(1, patient);*/
               /* Console.WriteLine($"{patient.Name} was created at {DateTimeOffset.Now}. URL is {url}");*/
                _logger.LogInformation($"Hello {count}");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
