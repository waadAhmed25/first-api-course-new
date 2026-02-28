namespace DNAAnalysis.Shared.Enums;

public enum RequestStatus
{
    Pending = 0,      // الطلب اتعمل ولسه متحللش
    Processing = 1,   // اتبعت للـ AI وبيتحلل
    Completed = 2,    // التحليل خلص والنتيجة جاهزة
    Failed = 3        // حصل خطأ في التحليل
}
