//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klevtsov_Zakharov
{
    using System;
    using System.Collections.Generic;
    
    public partial class AmentiesTickets
    {
        public int AmentiesId { get; set; }
        public int TicketId { get; set; }
        public Nullable<decimal> Price { get; set; }
    
        public virtual Amenties Amenties { get; set; }
        public virtual Tickets Tickets { get; set; }
    }
}
