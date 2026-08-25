import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { BackflowReasonForTest, BackflowTestResult } from "../../models/backflow/backflow-test";
import { PropertyType } from "../../models/sites/site";

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

    public readonly deviceTypeFormOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'DC', text: 'DC' },
        { id: 'DCD', text: 'DCD' },
        { id: 'DCD2', text: 'DCD2' },
        { id: 'RP', text: 'RP' },
        { id: 'RPPD', text: 'RPPD' },
        { id: 'RPPD2', text: 'RPPD2' },
        { id: 'PVB', text: 'PVB' },
        { id: 'SVB', text: 'SVB' },
        { id: 'AG', text: 'AG' }
    ];

    public readonly manufacturerOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'Ames', text: 'Ames' },
        { id: 'Apollo', text: 'Apollo' },
        { id: 'ARI', text: 'ARI' },
        { id: 'Buckner', text: 'Buckner' },
        { id: 'Cash Acme', text: 'Cash Acme' },
        { id: 'Cla-Val', text: 'Cla-Val' },
        { id: 'Conbraco', text: 'Conbraco' },
        { id: 'Backflow Direct', text: 'Backflow Direct' },
        { id: 'Febco', text: 'Febco' },
        { id: 'Flomatic', text: 'Flomatic' },
        { id: 'Hersey', text: 'Hersey' },
        { id: 'Neptune', text: 'Neptune' },
        { id: 'Watts', text: 'Watts' },
        { id: 'Wilkins', text: 'Wilkins' },
        { id: 'Other', text: 'Other' }
    ];

    public readonly sizeOptions: InputOption[] = [
        { id: '', text: '' },
        { id: '3/8', text: '3/8' },
        { id: '1/2', text: '1/2' },
        { id: '3/4', text: '3/4' },
        { id: '1', text: '1' },
        { id: '1 1/4', text: '1 1/4' },
        { id: '1 1/2', text: '1 1/2' },
        { id: '2', text: '2' },
        { id: '2 1/2', text: '2 1/2' },
        { id: '3', text: '3' },
        { id: '4', text: '4' },
        { id: '6', text: '6' },
        { id: '8', text: '8' },
        { id: '10', text: '10' },
        { id: '12', text: '12' },
        { id: '14', text: '14' },
        { id: '16', text: '16' }
    ];

    public readonly hazardTypeFormOptions: InputOption[] = this.hazardTypeOptions.filter(option => option.id !== '');

    public readonly scheduleMonthOptions: InputOption[] = [
        { id: '0', text: 'N/A' },
        { id: '1', text: '(1) January' },
        { id: '2', text: '(2) February' },
        { id: '3', text: '(3) March' },
        { id: '4', text: '(4) April' },
        { id: '5', text: '(5) May' },
        { id: '6', text: '(6) June' },
        { id: '7', text: '(7) July' },
        { id: '8', text: '(8) August' },
        { id: '9', text: '(9) September' },
        { id: '10', text: '(10) October' },
        { id: '11', text: '(11) November' },
        { id: '12', text: '(12) December' }
    ];

    public readonly forceRenewalYearsOptions: InputOption[] = [
        { id: '0', text: '6 Months' },
        { id: '1', text: '1 Year' },
        { id: '2', text: '2Years' },
        { id: '3', text: '3Years' },
        { id: '4', text: '4Years' },
        { id: '5', text: '5Years' }
    ];

    public readonly testResultFormOptions: InputOption[] = [
        { id: String(BackflowTestResult.Pass), text: 'Passed' },
        { id: String(BackflowTestResult.Fail), text: 'Failed' },
        { id: String(BackflowTestResult.PassAfterRepairs), text: 'Passed After Repairs' }
    ];

    public readonly reasonForTestFormOptions: InputOption[] = this.reasonForTestOptions.filter(option => option.id !== '');

    public readonly propertyTypeOptions: InputOption[] = [
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];
}