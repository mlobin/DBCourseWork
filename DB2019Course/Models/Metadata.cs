using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DB2019Course.Models
{
    public class AspirantMetadata
    {
        [Display(Name="Номер пропуска")]
        [Range(0, Int32.MaxValue,ErrorMessage ="Номер пропуска не может быть отрицательным!")]
        public int Pass;

        [Display(Name = "Имя")]
        [StringLength(20, ErrorMessage = "Имя не может быть длиннее 20 символов")]
        public string Name;

        [Display(Name = "Фамилия")]
        [StringLength(20, ErrorMessage = "Фамилия не может быть длиннее 20 символов")]
        public string LastName;

        [Display(Name = "Отчество")]
        [StringLength(20, ErrorMessage = "Отчество не может быть длиннее 20 символов")]
        public string SurName;

        [Display(Name = "Дата поступления")]
        [DataType(DataType.Date)]
        public DateTime JoinDate;

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate;

        [Display(Name = "Статус")]
        [StringLength(20, ErrorMessage = "Статус не может быть длиннее 20 символов")]
        public string Status;

        [Display(Name = "Дата выпуска")]
        [DataType(DataType.Date)]
        public DateTime OutDate;

        [Display(Name = "Группа")]
        public int GroupId;
    }

    public class LeaderMetadata
    {
        [Display(Name = "Номер пропуска")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int PassId;

        [Display(Name = "Имя")]
        [StringLength(20, ErrorMessage = "Имя не может быть длиннее 20 символов")]
        public string Name;

        [Display(Name = "Фамилия")]
        [StringLength(20, ErrorMessage = "Фамилия не может быть длиннее 20 символов")]
        public string Lastname;

        [Display(Name = "Отчество")]
        [StringLength(20, ErrorMessage = "Отчество не может быть длиннее 20 символов")]
        public string Surname;

        [Display(Name = "Кафедра")]
        [StringLength(20, ErrorMessage = "Аббревиатура кафедры не может быть длиннее 20 символов")]
        public string Department;

        [Display(Name = "Ученая степень")]
        [StringLength(20, ErrorMessage = "Ученая степень не может быть длиннее 20 символов")]
        public string Degree;

        [Display(Name = "Ученое звание")]
        [StringLength(20, ErrorMessage = "Ученое звание не может быть длиннее 20 символов")]
        public string Title;

        [Display(Name = "Должность")]
        [StringLength(20, ErrorMessage = "Должность не может быть длиннее 20 символов")]
        public string Position;
    }

    public class ExamMetadata
    {
        public int Id;

        [Display(Name = "Предмет")]
        [StringLength(20, ErrorMessage = "В названии предмета не может быть более 20 символов")]
        public string Subject;

        [Display(Name = "Дата экзамена")]
        [DataType(DataType.Date)]
        public DateTime Date;

        [Display(Name = "Аудитория")]
        [Range(1,1000,ErrorMessage = "Номер аудитории должен быть в интервале от 1 до 1000")]
        public int Auditory;

        [Display(Name = "Корпус")]
        [StringLength(20, ErrorMessage = "В названии корпуса не может быть более 20 символов")]
        public string Corp;

        [Display(Name = "Преподаватель")]
        [StringLength(20, ErrorMessage = "В имени преподавателя не может быть более 40 символов")]
        public string Teacher;
    }

    public class ResultMetadata
    {
        [Display(Name = "Аспирант")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int PassId;

        [Display(Name = "Экзамен")]
        public int ExamId;

        [Display(Name = "Результат экзамена")]
        [StringLength(20, ErrorMessage = "Результат экзамена не может быть длиннее 20 символов")]
        public string Result1;
    }

    public class WorkMetadata
    {
        public int Id;

        [Display(Name = "Автор")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Название работы")]
        [StringLength(40, ErrorMessage = "Название работы не может быть длиннее 40 символов")]
        public string Name;

        [Display(Name = "Направление работы")]
        [StringLength(40, ErrorMessage = "Направление работы не может быть длиннее 40 символов")]
        public string Theme;

        [Display(Name = "Кафедра")]
        [StringLength(20, ErrorMessage = "Аббревиатура кафедры не может быть длиннее 20 символов")]
        public string Department;

        [Display(Name = "УДК")]
        [StringLength(40, ErrorMessage = "УДК не может быть длиннее 40 символов")]
        public string UDC;
    }

    public class ArticleMetadata
    {
        public int Id;

        [Display(Name = "Автор")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Журнал")]
        [StringLength(20, ErrorMessage = "В названии журнала не может быть более 20 символов")]
        public string Journal;

        [Display(Name = "Номер")]
        [StringLength(20, ErrorMessage = "В номере не может быть более 20 символов")]
        public string Issue;

        [Display(Name = "ISBN")]
        [StringLength(13, MinimumLength =13, ErrorMessage ="ISBN должен состоять из 13 цифр!")]
        [RegularExpression("([0-9]+)",ErrorMessage = "ISBN может содержать только цифры!")]
        public string ISBN;

        [Display(Name = "Соавтор")]
        public int? CoauthorPass;

        [Display(Name = "Год выпуска")]
        [Range(1990, 2100, ErrorMessage = "Некорректный год выпуска!")]
        public int Year;
    }

    public class DisserMetadata
    {
        public int Id;

        [Display(Name = "Автор")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Номенклатура")]
        [StringLength(20, ErrorMessage = "Номенклатура не может быть длиннее 20 символов")]
        public string Nomenclature;

        [Display(Name = "Готовность")]
        [Range(0, 100, ErrorMessage = "Готовность описывается цифрой от 0 до 100%")]
        public int Readiness;
    }

    public class GroupMetadata
    {
        public int Id;

        [Display(Name = "Факультет")]
        [StringLength(20, ErrorMessage = "Аббревиатура факультета не может быть длиннее 20 символов")]
        public string Faculty;

        [Display(Name = "Курс")]
        [Range(1, 3, ErrorMessage = "Курс может варьироваться от 1 до 3")]
        public byte Stage;

        [Display(Name = "Название")]
        [StringLength(40, ErrorMessage = "название не может быть длиннее 40 символов")]
        public string Name;

        [Display(Name = "Направление")]
        [StringLength(40, ErrorMessage = "Направление не может быть длиннее 40 символов")]
        public string Specialty;
    }

    public class IndPlanMetadata
    {
        public int Id;

        [Display(Name = "Аспирант")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AspirantId;

        [Display(Name = "Название элемента")]
        [StringLength(20, ErrorMessage = "Элемент не может быть длиннее 20 символов")]
        public string WorkName;

        [Display(Name = "Объем")]
        [Range(0, 100, ErrorMessage = "Объем описывается цифрой от 0 до 100 ак. часов")]
        public int Size;

        [Display(Name = "Дата сдачи")]
        [DataType(DataType.Date)]
        public System.DateTime DueDate;

        [Display(Name = "Показатель завершенности")]
        [StringLength(20, ErrorMessage = "Показатель не может быть длиннее 20 символов")]
        public string DoneMarker;

        [Display(Name = "Готовность")]
        [StringLength(20, ErrorMessage = "Готовность не может быть длиннее 20 символов")]
        public string Readiness;

        [Display(Name = "Год обучения")]
        [Range(1990, 2100, ErrorMessage = "Некорректный год обучения!")]
        public int Year;
    }

    public class DefenceMetadata
    {
        
        [Display(Name = "Номер защиты")]
        [Range(0,int.MaxValue,ErrorMessage = "Номер защиты не может быть отрицательным!")]
        public int Number;

        [Display(Name = "Пропуск автора")]
        [Range(0, int.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Диссертация")]
        public int DisserId;

        [Display(Name = "Дата защиты")]
        [DataType(DataType.Date)]
        public System.DateTime Date;

        [Display(Name = "Первый оппонент")]
        [StringLength(40, ErrorMessage ="Имя оппонента не может быть длиннее 40 символов!")]
        public string Opponent;

        [Display(Name = "Второй оппонент")]
        [StringLength(40, ErrorMessage = "Имя оппонента не может быть длиннее 40 символов!")]
        public string Opponent2;

        [Display(Name = "Результат защиты")]
        [StringLength(20, ErrorMessage = "Результат защиты не может быть длиннее 20 символов!")]
        public string Result;

        [StringLength(20, ErrorMessage ="название города не может быть длиннее 20 символов!")]
        [Display(Name = "Город")]
        public string City;

        [Display(Name = "Защищающая организация")]
        [StringLength(40, ErrorMessage ="Название защищающей организации не может быть длиннее 40 символов!")]
        public string Organisation;

        [Display(Name = "Здания")]
        [StringLength(20, ErrorMessage ="Название здания не может быть длиннее 20 символов!")]
        public string Building;

        [Display(Name = "Аудитория")]
        [StringLength(20, ErrorMessage ="Номер аудитории не может быть длиннее 20 символов!")]
        public string Auditory;

    }

    public class ReviewMetadata
    {
        public int Id;

        [Display(Name = "Пропуск автора")]
        [Range(0, int.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Диссертация")]
        public int DisserId;

        [Display(Name = "Автор отзыва")]
        [StringLength(40, ErrorMessage = "Имя автора отзыва не может быть длиннее 40 символов!")]
        public string ReviewAuthor;

        [Display(Name = "Организация")]
        [StringLength(40, ErrorMessage = "Название организации не может быть длиннее 40 символов!")]
        public string Organisation;
    }

    public class VotingMetadata
    {
        [Display(Name = "Номер совета")]
        [StringLength(20, ErrorMessage = "Номер совета не может быть длиннее 20 символов!")]
        public string VoteNumber;

        [Display(Name = "Количество членов")]
        [Range(7, 140, ErrorMessage = "Количество членов должно быть в интервале от 7 до 140!")]
        public int Members;

        [Display(Name = "Явка")]
        [Range(0, 140, ErrorMessage = "Явка должна быть в интервале от 0 до максимума членов совета!")]
        public int Arrived;

        [Display(Name = "Голосовали \"За\"")]
        [Range(0, 140, ErrorMessage = "Количество голосовавших \"За\" может быть от 0 до явки")]
        public int Pro;

        [Display(Name = "Голосовали \"Против\"")]
        [Range(0, 140, ErrorMessage = "Количество голосовавших \"Против\" может быть от 0 до явки")]
        public int Contra;

        [Display(Name = "Недействительных бюллетеней")]
        [Range(0, 140, ErrorMessage = "Количество недействительных бюллетеней не может быть отрицательным и не может быть больше явки!")]
        public int Not_Voted;

        [Display(Name = "Диссертация")]
        [Range(0, int.MaxValue, ErrorMessage = "Номер диссертации не может быть отрицательным!")]
        public int DisserId;

        [Display(Name = "Автор")]
        [Range(0, int.MaxValue, ErrorMessage = "Номер пропуска не может быть отрицательным!")]
        public int AuthorPass;

        [Display(Name = "Номер защиты")]
        [Range(0, int.MaxValue, ErrorMessage = "Номер защиты не может быть отрицательным!")]
        public int DefenceId;
    }
}