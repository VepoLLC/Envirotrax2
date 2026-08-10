import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { BackflowReasonForTest } from "../../models/backflow/backflow-test";

@Injectable({
    providedIn: 'root'
})
export class BackflowTestOptionsService {
    public readonly deviceTypeOptions: InputOption[] = [
        { id: '', text: 'All Device Types' },
        { id: 'DC', text: 'DC - Double Check Valve' },
        { id: 'DCD', text: 'DCD - Double Check Detector' },
        { id: 'DCD2', text: 'DCD2 - Double Check Detector Type II' },
        { id: 'RP', text: 'RP - Reduced Pressure Principle' },
        { id: 'RPPD', text: 'RPPD - Reduced Pressure Principle Detector' },
        { id: 'RPPD2', text: 'RPPD2 - Reduced Pressure Principle Detector Type II' },
        { id: 'PVB', text: 'PVB - Pressure Vacuum Breaker' },
        { id: 'SVB', text: 'SVB - Spill-Resistant Pressure Vacuum Breaker' },
        { id: 'AG', text: 'AG - Air Gap' }
    ];

    public readonly hazardTypeOptions: InputOption[] = [
        { id: '', text: 'All Hazard Types' },
        { id: 'Agricultural/Feed Lot', text: 'Agricultural/Feed Lot' },
        { id: 'Domestic/Premises Isolation', text: 'Domestic/Premises Isolation' },
        { id: 'Fire System', text: 'Fire System' },
        { id: 'Gas Station/Car Wash', text: 'Gas Station/Car Wash' },
        { id: 'Irrigation - Non Chemical', text: 'Irrigation - Non Chemical' },
        { id: 'Irrigation - Chemical Feed', text: 'Irrigation - Chemical Feed' },
        { id: 'Laundry/Cleaners', text: 'Laundry/Cleaners' },
        { id: 'Medical/Dental/Laboratory/Mortuary', text: 'Medical/Dental/Laboratory/Mortuary' },
        { id: 'Nails/Salon/Grooming', text: 'Nails/Salon/Grooming' },
        { id: 'Pool/Recreation/Athletics', text: 'Pool/Recreation/Athletics' },
        { id: 'Restaurant/Vending/Grocery', text: 'Restaurant/Vending/Grocery' },
        { id: 'Fire Hydrant/Temporary Construction', text: 'Fire Hydrant/Temporary Construction' },
        { id: 'Fountains/Garden Ponds/Water Features', text: 'Fountains/Garden Ponds/Water Features' },
        { id: 'Water Softener', text: 'Water Softener' },
        { id: 'Other', text: 'Other' }
    ];

    public readonly reasonForTestOptions: InputOption[] = [
        { id: '', text: 'All Values' },
        { id: String(BackflowReasonForTest.AnnualTest), text: 'Annual Test' },
        { id: String(BackflowReasonForTest.NewInstallation), text: 'New Installation' },
        { id: String(BackflowReasonForTest.ExistingInstallation), text: 'Existing Installation' },
        { id: String(BackflowReasonForTest.Replacement), text: 'Replacement' },
        { id: String(BackflowReasonForTest.Repair), text: 'Repair' },
        { id: String(BackflowReasonForTest.AnnualTestAfterRepairs), text: 'Annual Test After Repairs' }
    ];
}
