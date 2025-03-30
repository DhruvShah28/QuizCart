namespace QuizCart.Models.ViewModels
{
    public class MemberDetails
    {
        public MemberDto Member { get; set; }

        public List<SubjectDto>? Subjects { get; set; }
        public List<PurchasesDto>? Purchases { get; set; }
        public List<AssessmentDto>? Assessments { get; set; }
        public List<SubjectDto>? UnlinkedSubjects { get; set; }

    }
}
