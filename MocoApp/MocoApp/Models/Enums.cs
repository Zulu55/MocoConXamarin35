namespace MocoApp.Models
{
    public static class Enums
    {
        public enum UserRole
        {
            Admin = 1, //Administrador
            Manager = 2, //Gerente
            Employee = 3, //Funcionario
            Client = 4, //Cliente
            Guest = 5
        }

        public enum NotificationType
        {
            CheckinSent = 1, //Solicitação de checkin enviada
            OrderStatusChanged = 2, //Status de pedido mudado
        }

        public enum DesirableLanguage
        {
            Portuguese = 1,
            English = 2,
            Spanish = 3
        }

        public enum OrderStatus
        {
            Pending = 1,//Pendente
            Accepted = 2, //Aceitado
            OnGoing = 3, //Em andamento
            Canceled = 4, //Cancelado
            Done = 5, //Entregue
            Closed = 6 //Finalizado
        }

        public enum ClientCompanyStatus
        {
            CheckinPending = 1,
            Checkin = 2,
            RequestedCheckout = 3,
            Checkout = 4,
            RequestedCheckin = 5,
            Denied = 6
        }

        public enum CompanyType
        {
            Hotel = 1,
            Restaurante = 2,
            Praia = 3,
            EsporteEvento = 4
        }

        public enum ClientStatus
        {
            NaoCadastrado = 1,
            Cadastrado = 2
        }

        public enum FilterType
        {
            Proximidade = 1,
            OrderByName = 2,
            Favorites = 3,
            Ratings = 4
        }

        public enum CheckinStatus
        {
            CheckinPending = 1, // estab cadastro o cliente (n usa mais)
            Checkin = 2, //checkin sucessido no estab
            RequestedCheckout = 3, //pediu a conta
            Checkout = 4, //conta fechada - so pra registro
            RequestedCheckin = 5, //cliente pediu cadastorno estabelecimento 
            Denied = 6 // checkin negado
        }

        public enum CheckinSubStatus
        {
            Undefined = 0,
            Pending = 1,
            Active = 2, //SubCheckin ativo no checkin
            Closed = 3, //SubCheckin já encerrado 
            RequestedCheckout = 4,
            Checkout = 5, //conta fechada - so pra registro
            Denied = 6
        }

        public enum LocationType
        {
            Undefined = 0,
            Room = 1,
            Bar = 2,
            Restaurant = 3,
            MeetingRoom = 4,
            Spa = 5,
            Valet = 6,
            Concierge = 7,
            SwimmingPool = 8
        }

        public enum PaymentMethod
        {
            NotInformed = 0,
            Card = 1,
            Money = 2,
            CreditCard = 3,
            Transferred = 4,
            BillStillOpen = 5,
            PayLater = 99
        }

        public enum EMenuType
        {
            Undefined = 0,
            Category = 1,
            Informative = 2,
            InformativeWithoutPrice = 3
        }

        public enum ECurrencyType
        {
            USD = 0,
            BRT = 1,
            EUR = 2,
            COL = 3
        }
    }
}