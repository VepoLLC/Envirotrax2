import { Injectable } from "@angular/core";
import { BackflowDeviceType, BackflowReasonForTest, BackflowTestResult } from "../../models/backflow/backflow-test-enums";
import { InputOption } from "@envirotrax/common-ui";

@Injectable({
    providedIn: 'root'
})
export class BackflowTestOptionsService {
    public readonly deviceTypeOptions: InputOption[] = [
        { id: BackflowDeviceType.DC, text: 'DC - Double Check Valve' },
        { id: BackflowDeviceType.DCD, text: 'DCD - Double Check Detector' },
        { id: BackflowDeviceType.DCD2, text: 'DCD2 - Double Check Detector Type II' },
        { id: BackflowDeviceType.RP, text: 'RP - Reduced Pressure Principle' },
        { id: BackflowDeviceType.RPPD, text: 'RPPD - Reduced Pressure Principle Detector' },
        { id: BackflowDeviceType.RPPD2, text: 'RPPD2 - Reduced Pressure Principle Detector Type II' },
        { id: BackflowDeviceType.PVB, text: 'PVB - Pressure Vacuum Breaker' },
        { id: BackflowDeviceType.SVB, text: 'SVB - Spill-Resistant Pressure Vacuum Breaker' },
        { id: BackflowDeviceType.AG, text: 'AG - Air Gap' }
    ];

    public readonly deviceTypeFilterOptions: InputOption[] = [
        { id: '', text: 'All Device Types' },
        ...this.deviceTypeOptions
    ];

    public readonly hazardTypeOptions: InputOption[] = [
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

    public readonly hazardTypeFilterOptions: InputOption[] = [
        { id: '', text: 'All Hazard Types' },
        ...this.hazardTypeOptions
    ];

    public readonly reasonOptions: InputOption[] = [
        { id: BackflowReasonForTest.AnnualTest, text: 'Annual Test' },
        { id: BackflowReasonForTest.NewInstallation, text: 'New Installation' },
        { id: BackflowReasonForTest.ExistingInstallation, text: 'Existing Installation' },
        { id: BackflowReasonForTest.Replacement, text: 'Replacement' },
        { id: BackflowReasonForTest.Repair, text: 'Repair' },
        { id: BackflowReasonForTest.AnnualTestAfterRepairs, text: 'Annual Test After Repairs' }
    ];

    public readonly reasonFilterOptions: InputOption[] = [
        { id: '', text: 'All Values' },
        ...this.reasonOptions.map(o => ({ id: o.id?.toString(), text: o.text }))
    ];

    public readonly testResultOptions: InputOption[] = [
        { id: '', text: 'All Test Results' },
        { id: BackflowTestResult.Pass.toString(), text: 'Pass' },
        { id: BackflowTestResult.Fail.toString(), text: 'Fail' },
        { id: BackflowTestResult.PassAfterRepairs.toString(), text: 'Pass After Repairs' }
    ];

    public readonly approvalStatusOptions: InputOption[] = [
        { id: '', text: 'Any Status' },
        { id: 'false', text: 'Approved' },
        { id: 'true', text: 'Disapproved' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any Status' },
        { id: 'true', text: 'Paid' },
        { id: 'false', text: 'Unpaid' }
    ];
}
