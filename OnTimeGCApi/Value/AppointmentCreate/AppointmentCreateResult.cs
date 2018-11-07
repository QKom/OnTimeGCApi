
namespace OnTimeGCApi
{
    public class AppointmentCreateResult : BaseResult
    {
        public AppointmentCreate AppointmentCreate { get; set; }

        public override string ToString()
        {
            return $"{{ Status: \"{this.Status}\" | IsAnonymous: \"{this.IsAnonymous}\" | ErrorCode: \"{this.ErrorCode}\" | AppointmentCreate: {this.AppointmentCreate} }}";
        }
    }
}