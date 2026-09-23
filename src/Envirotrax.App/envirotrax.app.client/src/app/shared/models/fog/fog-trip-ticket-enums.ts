export enum FogTripTicketStatus {
    PickupNotCompleted = '1',
    PickupCompleted = '2',
    TripTicketCompleted = '3'
}

export enum FogTripTicketPaymentStatus {
    Unpaid = 'Unpaid',
    Paid = 'Paid'
}

export enum FogTripTicketApprovalStatus {
    Approved = 'Approved',
    Disapproved = 'Disapproved'
}

export enum FogTripTicketDateType {
    RecordCreationDate = 'recordCreationDate',
    WasteRemovalDate = 'interceptorWasteRemovedDate',
    WasteDeliveredDate = 'receiverWasteDeliveredDate'
}
