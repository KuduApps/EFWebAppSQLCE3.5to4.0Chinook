//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Chinook.Model
{
    public partial class InvoiceLine
    {
        #region Primitive Properties
    
        public virtual int InvoiceLineId
        {
            get;
            set;
        }
    
        public virtual int InvoiceId
        {
            get { return _invoiceId; }
            set
            {
                if (_invoiceId != value)
                {
                    if (Invoice != null && Invoice.InvoiceId != value)
                    {
                        Invoice = null;
                    }
                    _invoiceId = value;
                }
            }
        }
        private int _invoiceId;
    
        public virtual int TrackId
        {
            get { return _trackId; }
            set
            {
                if (_trackId != value)
                {
                    if (Track != null && Track.TrackId != value)
                    {
                        Track = null;
                    }
                    _trackId = value;
                }
            }
        }
        private int _trackId;
    
        public virtual decimal UnitPrice
        {
            get;
            set;
        }
    
        public virtual int Quantity
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual Invoice Invoice
        {
            get { return _invoice; }
            set
            {
                if (!ReferenceEquals(_invoice, value))
                {
                    var previousValue = _invoice;
                    _invoice = value;
                    FixupInvoice(previousValue);
                }
            }
        }
        private Invoice _invoice;
    
        public virtual Track Track
        {
            get { return _track; }
            set
            {
                if (!ReferenceEquals(_track, value))
                {
                    var previousValue = _track;
                    _track = value;
                    FixupTrack(previousValue);
                }
            }
        }
        private Track _track;

        #endregion
        #region Association Fixup
    
        private void FixupInvoice(Invoice previousValue)
        {
            if (previousValue != null && previousValue.InvoiceLines.Contains(this))
            {
                previousValue.InvoiceLines.Remove(this);
            }
    
            if (Invoice != null)
            {
                if (!Invoice.InvoiceLines.Contains(this))
                {
                    Invoice.InvoiceLines.Add(this);
                }
                if (InvoiceId != Invoice.InvoiceId)
                {
                    InvoiceId = Invoice.InvoiceId;
                }
            }
        }
    
        private void FixupTrack(Track previousValue)
        {
            if (previousValue != null && previousValue.InvoiceLines.Contains(this))
            {
                previousValue.InvoiceLines.Remove(this);
            }
    
            if (Track != null)
            {
                if (!Track.InvoiceLines.Contains(this))
                {
                    Track.InvoiceLines.Add(this);
                }
                if (TrackId != Track.TrackId)
                {
                    TrackId = Track.TrackId;
                }
            }
        }

        #endregion
    }
}
