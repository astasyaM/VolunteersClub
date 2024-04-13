using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;
using VolunteersClub.Data;
using VolunteersClub.Models;


namespace VolunteersClub.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GenerateReport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Events.AsQueryable();

            // Применяем фильтр по дате начала, если указана
            if (endDate.HasValue)
            {
                query = query.Where(p => p.EventDate.Year < endDate.Value.Year ||
                    p.EventDate.Year == endDate.Value.Year && p.EventDate.Month < endDate.Value.Month ||
                    p.EventDate.Year == endDate.Value.Year && p.EventDate.Month == endDate.Value.Month && p.EventDate.Day < endDate.Value.Day);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.EventDate.Year > startDate.Value.Year ||
                    p.EventDate.Year == startDate.Value.Year && p.EventDate.Month > startDate.Value.Month ||
                    p.EventDate.Year == startDate.Value.Year && p.EventDate.Month == startDate.Value.Month && p.EventDate.Day > startDate.Value.Day);
            }

            var events = query.ToList();

            var eventsID = events
                .Select(e => e.EventID)
                .ToList();

            var eventTypesData = _context.Events
                .Where(e => events.Contains(e))
                .GroupBy(e => e.EventTypeID)
                .Select(g => new
                {
                    Type = _context.EventTypes.FirstOrDefault(t => t.EventTypeID == g.Key).EventTypeName,
                    Count = g.Count(),
                    Percentage = Math.Round(((double)g.Count() / events.Count) * 100, 2)
                })
                .ToList();

            var volunteers = _context.Volunteers.ToList();
            int volunteersTrainy = volunteers
                .Where(e => e.VolunteerStatusID == 1)
                .Count();

            int volunteersSeniors = volunteers
                .Where(e => e.VolunteerStatusID == 2)
                .Count();

            int amountParticipants = _context.Participants
                .Where(e => eventsID.Contains(e.EventID)
                    && e.ConfirmedLeader == true && e.ConfirmedVolunteer == true)
                .Select(e => e.VolunteerID)
                .Distinct()
                .Count();

            var participantsID = _context.Participants
                .Where(e => eventsID.Contains(e.EventID)
                    && e.ConfirmedLeader == true && e.ConfirmedVolunteer == true)
                .Select(e => e.RecordID)
                .ToList();

            // Получаем данные о количестве волонтёров, необходимых для каждого мероприятия
            var eventsParticipants = _context.Events
                .Where(e=>eventsID.Contains(e.EventID))
                .Select(e => new
            {
                EventId = e.EventID,
                EventName = e.EventName,
                Number = e.VolunteersNumber
            }).ToList();

            // Получаем данные о количестве волонтёров, принявших участие в каждом мероприятии
            var participantsCount = _context.Participants
                .Where(e => eventsID.Contains(e.EventID)
                    && e.ConfirmedLeader == true && e.ConfirmedVolunteer == true)
                .GroupBy(p => p.EventID)
                .Select(g => new
                {
                    EventId = g.Key,
                    ParticipantsCount = g.Count()
                }).ToList();

            // Вычисляем процент участия волонтеров от запланированного количества для каждого мероприятия
            var participationPercentages = eventsParticipants
                .Select(e => new
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    ParticipationPercentage = participantsCount
                        .Where(p => p.EventId == e.EventId)
                        .Select(p => (double)p.ParticipantsCount / e.Number * 100)
                        .FirstOrDefault()
                }).ToList();

            // Получаем список всех волонтёров и их оценок за участие
            var volunteersMarks = _context.Marks
                .Where(m => participantsID.Contains(m.ActivityRecordID))
                .Select(g => g.CurrentMark)
                .ToList();

            // Находим максимальную оценку
            var maxMark = volunteersMarks.Max();

            // Выбираем волонтёров с максимальной оценкой
            var topEventsID = _context.Marks
                .Where(vm => vm.CurrentMark == maxMark)
                .Select(vm => vm.ActivityRecordID)
                .ToList();

            var topVolunteersID = _context.Participants
                .Where(e => topEventsID.Contains(e.RecordID))
                .Select(v => v.VolunteerID)
                .ToList();

            var topVolunteers = _context.Volunteers
                .Where(e => topVolunteersID.Contains(e.VolunteerID))
                .ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy"));

                if (startDate.HasValue && endDate.HasValue)
                {
                    worksheet.Cells["A1"].Value = $"Период отчёта: {startDate?.ToString("dd.MM.yyyy")} - {endDate?.ToString("dd.MM.yyyy")}";
                }

                worksheet.Cells["A2"].Value = "Количество проведённых мероприятий:";
                worksheet.Cells["B2"].Value = events.Count();

                worksheet.Cells["A3"].Value = "Список проведённых мероприятий:";
                int row = 4;
                for (int i = 0; i < events.Count; i++)
                {
                    worksheet.Cells[i + 4, 1].Value = events[i].EventName;
                    row += 1;
                }

                row += 1;
                // Диаграмма
                // Добавляем заголовок для круговой диаграммы
                worksheet.Cells["A" + row].Value = "Тип мероприятия";
                worksheet.Cells["B" + row].Value = "Процент";

                // Заполняем данные для круговой диаграммы
                row += 1;
                int countRow = 0;
                foreach (var item in eventTypesData)
                {
                    worksheet.Cells["A" + row].Value = item.Type;
                    worksheet.Cells["B" + row].Value = item.Percentage;
                    row++;
                    countRow += 1;
                }

                // Создаем круговую диаграмму
                var chart = worksheet.Drawings.AddChart("PieChart", OfficeOpenXml.Drawing.Chart.eChartType.Pie);
                chart.SetPosition(1, 0, 3, 0);
                chart.SetSize(500, 400);
                chart.Series.Add(worksheet.Cells[$"B{row - countRow}:B{row - 1}"], worksheet.Cells[$"A{row - countRow}:A{row - 1}"]);
                chart.Title.Text = "Процентное соотношение типов проведённых мероприятий";

                row += 1;
                // Общее количество волонтёров
                worksheet.Cells["A" + row].Value = "Количество участников центра:";
                worksheet.Cells["B" + row].Value = volunteersSeniors+volunteersTrainy;

                row += 1;
                // Количество волонтёров в разных статусах
                worksheet.Cells["A" + row].Value = "Количество волонтёров в центре:";
                worksheet.Cells["B" + row].Value = volunteersSeniors;
                row += 1;
                worksheet.Cells["A" + row].Value = "Количество стажёров в центре:";
                worksheet.Cells["B" + row].Value = volunteersTrainy;
                row += 1;
                worksheet.Cells["A" + row].Value = "Кол-во волонтёров, участвовавших в мероприятиях:";
                worksheet.Cells["B" + row].Value = amountParticipants;

                // Список лучших волонтёров и их оценки
                row += 2;
                worksheet.Cells["A" + row].Value = "Лучшие волонтёры:";
                worksheet.Cells["B" + row].Value = "Наивысшая оценка:";
                row += 1;
                foreach (var volunteer in topVolunteers)
                {
                    worksheet.Cells["A" + row].Value = volunteer.Name+" "+volunteer.Surname;
                    worksheet.Cells["B" + row].Value = maxMark;
                    row++;
                }

                row += 2;
                //столбчатая диаграмма для % заполняемости
                worksheet.Cells["A" + row].Value = "Название мероприятия";
                worksheet.Cells["B" + row].Value = "Процент участия";

                // Заполняем данные для столбчатой диаграммы
                row += 1;
                countRow = 0;
                foreach (var item in participationPercentages)
                {
                    worksheet.Cells["A" + row].Value = item.EventName;
                    worksheet.Cells["B" + row].Value = item.ParticipationPercentage;
                    row++;
                    countRow += 1;
                }

                // Добавляем столбчатую диаграмму
                var chartNew = worksheet.Drawings.AddChart("BarChart", OfficeOpenXml.Drawing.Chart.eChartType.BarClustered);
                chartNew.SetPosition(23, 0, 3, 0);
                chartNew.SetSize(500, 200);
                chartNew.Series.Add(worksheet.Cells["B" + (row - countRow) + ":B" + (row - 1)], worksheet.Cells["A" + (row - countRow) + ":A" + (row - 1)]);
                chartNew.Title.Text = "Выполнение плана по кол-ву участников на мероприятие";

                worksheet.Cells.AutoFitColumns();

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
            }
        }

    }
}

