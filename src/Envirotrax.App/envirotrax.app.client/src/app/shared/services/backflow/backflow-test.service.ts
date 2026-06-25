import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { BackflowTest } from "../../models/backflow/backflow-test";
import { BackflowTestImages } from "../../models/backflow/backflow-test-images";
import { DownloadEndpoint } from "../../models/download-config";

export type BackflowExpiryRangeKey = 'expired' | 'thismonth' | 'nextmonth' | 'twomonths';

export function getBackflowExpiryRange(key: BackflowExpiryRangeKey): { start: Date; end: Date } {
    const now = new Date();

    if (key === 'expired') {
        const start = new Date(now);
        start.setMonth(start.getMonth() - 6);
        return { start, end: now };
    }

    const offset = key === 'thismonth' ? 0 : key === 'nextmonth' ? 1 : 2;
    const start = new Date(now.getFullYear(), now.getMonth() + offset, 1, 0, 0, 0, 0);
    const end = new Date(now.getFullYear(), now.getMonth() + offset + 1, 0, 23, 59, 59, 999);
    return { start, end };
}

@Injectable({
    providedIn: 'root'
})
export class BackflowTestService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowTest>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/tests');

        const observable = this._http.get<PagedData<BackflowTest>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public getAllEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/backflow/tests')
        };
    }

    public getAllPdfEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/backflow/tests/pdf')
        };
    }

    public getAllForProfessionalEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/professionals/backflow/tests')
        };
    }

    public getAllForProfessionalPdfEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/professionals/backflow/tests/pdf')
        };
    }

    public async getAllForProfessional(pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowTest>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/tests');

        return await lastValueFrom(this._http.get<PagedData<BackflowTest>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async submit(test: BackflowTest, images: BackflowTestImages = {}): Promise<BackflowTest> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/tests');
        const formData = buildBackflowTestFormData(test);
        
        if (images.assemblyImage) { formData.append('assemblyImage', images.assemblyImage); }
        if (images.serialNumberImage) { formData.append('serialNumberImage', images.serialNumberImage); }
        if (images.bypassAssemblyImage) { formData.append('bypassAssemblyImage', images.bypassAssemblyImage); }
        if (images.bypassSerialNumberImage) { formData.append('bypassSerialNumberImage', images.bypassSerialNumberImage); }
        if (images.airGapImage) { formData.append('airGapImage', images.airGapImage); }

        return await lastValueFrom(this._http.post<BackflowTest>(url, formData));
    }

    public async get(id: number): Promise<BackflowTest> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}`);
        return await lastValueFrom(this._http.get<BackflowTest>(url));
    }

    public async getPdf(id: number): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}/pdf`);
        return await lastValueFrom(this._http.get(url, { responseType: 'blob' }));
    }

    public async update(test: BackflowTest): Promise<BackflowTest> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${test.id}`);
        return await lastValueFrom(this._http.put<BackflowTest>(url, test));
    }

    public async uploadImage(id: number, imageType: string, file: File): Promise<BackflowTest> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}/images/${imageType}`);
        const formData = new FormData();
        formData.append('file', file);
        return await lastValueFrom(this._http.post<BackflowTest>(url, formData));
    }

    public async getForProfessional(id: number): Promise<BackflowTest> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/tests/${id}`);
        return await lastValueFrom(this._http.get<BackflowTest>(url));
    }

    public async getPdfForProfessional(id: number): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/tests/${id}/pdf`);
        return await lastValueFrom(this._http.get(url, { responseType: 'blob' }));
    }
}

function buildBackflowTestFormData(test: BackflowTest): FormData {
    const fd = new FormData();

    const append = (key: string, val: unknown): void => {
        if (val !== null && val !== undefined) {
            fd.append(key, String(val));
        }
    };

    // Nested reference IDs — dot-notation maps to ASP.NET Core model binding
    if (test.waterSupplier?.id != null) { fd.append('waterSupplier.id', String(test.waterSupplier.id)); }
    if (test.site?.id != null) { fd.append('site.id', String(test.site.id)); }
    if (test.bpat?.id != null) { fd.append('bpat.id', String(test.bpat.id)); }
    if (test.propertyState?.id != null) { fd.append('propertyState.id', String(test.propertyState.id)); }
    if (test.mailingState?.id != null) { fd.append('mailingState.id', String(test.mailingState.id)); }

    // Submission
    append('submissionId', test.submissionId);
    append('jobNumber', test.jobNumber);

    // BPAT
    append('bpatLicenseNumber', test.bpatLicenseNumber);
    append('bpatLicenseExpiration', test.bpatLicenseExpiration);

    // Property
    append('accountNumber', test.accountNumber);
    append('propertyBusinessName', test.propertyBusinessName);
    append('propertyType', test.propertyType);
    append('propertyStreetNumber', test.propertyStreetNumber);
    append('propertyStreetName', test.propertyStreetName);
    append('propertyNumber', test.propertyNumber);
    append('propertyCity', test.propertyCity);
    append('propertyZip', test.propertyZip);

    // Mailing
    append('mailingCompanyName', test.mailingCompanyName);
    append('mailingContactName', test.mailingContactName);
    append('mailingStreetNumber', test.mailingStreetNumber);
    append('mailingStreetName', test.mailingStreetName);
    append('mailingNumber', test.mailingNumber);
    append('mailingCity', test.mailingCity);
    append('mailingZip', test.mailingZip);
    append('mailingPhoneNumber', test.mailingPhoneNumber);
    append('mailingEmailAddress', test.mailingEmailAddress);

    // Device
    append('deviceType', test.deviceType);
    append('manufacturer', test.manufacturer);
    append('model', test.model);
    append('size', test.size);
    append('serialNumber', test.serialNumber);
    append('unknownSerialNumber', test.unknownSerialNumber);
    append('manufacturer2', test.manufacturer2);
    append('model2', test.model2);
    append('size2', test.size2);
    append('serialNumber2', test.serialNumber2);

    // Hazard & location
    append('locationDescription', test.locationDescription);
    append('hazardType', test.hazardType);
    append('hazardTypeOtherDescription', test.hazardTypeOtherDescription);

    // Test info
    append('reasonForTest', test.reasonForTest);
    append('replacementAssembly', test.replacementAssembly);
    append('installationDate', test.installationDate);
    append('testDate', test.testDate);
    append('initialTestDate', test.initialTestDate);
    append('repairTestDate', test.repairTestDate);
    append('finalTestDate', test.finalTestDate);
    append('expirationDate', test.expirationDate);
    append('testResult', test.testResult);
    append('properlyInstalled', test.properlyInstalled);
    append('nonPotable', test.nonPotable);

    // Gauge
    append('gaugeManufacturer', test.gaugeManufacturer);
    append('gaugeModel', test.gaugeModel);
    append('gaugeSerialNumber', test.gaugeSerialNumber);
    append('gaugeLastCalibrationDate', test.gaugeLastCalibrationDate);
    append('gaugeNonPotable', test.gaugeNonPotable);

    // Meter
    append('meterNumber', test.meterNumber);
    append('meterReadingBefore', test.meterReadingBefore);
    append('meterRegisters', test.meterRegisters);
    append('meterReadingAfter', test.meterReadingAfter);

    // Permit
    append('permitNumber', test.permitNumber);
    append('ossf', test.ossf);
    append('rainFreezeSensorInstalled', test.rainFreezeSensorInstalled);
    append('rainFreezeSensorWorkingProperly', test.rainFreezeSensorWorkingProperly);

    // Comments
    append('comments', test.comments);

    // Initial test readings — main assembly
    append('initCV1HeldPSID', test.initCV1HeldPSID);
    append('initCV1ClosedTight', test.initCV1ClosedTight);
    append('initCV1Leaked', test.initCV1Leaked);
    append('initCV2HeldPSID', test.initCV2HeldPSID);
    append('initCV2ClosedTight', test.initCV2ClosedTight);
    append('initCV2Leaked', test.initCV2Leaked);
    append('initRVOpenedPSID', test.initRVOpenedPSID);
    append('initRVDidNotOpen', test.initRVDidNotOpen);
    append('initBCHeldPSID', test.initBCHeldPSID);
    append('initBCClosedTight', test.initBCClosedTight);
    append('initBCLeaked', test.initBCLeaked);
    append('initPvbAirInletOpenedPSID', test.initPvbAirInletOpenedPSID);
    append('initPvbAirInletDidNotOpen', test.initPvbAirInletDidNotOpen);
    append('initPvbAirInletFullyOpened', test.initPvbAirInletFullyOpened);
    append('initPvbCVHeldPSID', test.initPvbCVHeldPSID);
    append('initPvbCVLeaked', test.initPvbCVLeaked);
    append('airGapValid', test.airGapValid);

    // Repairs — main assembly
    append('repairCV1', test.repairCV1);
    append('repairCV1Details', test.repairCV1Details);
    append('repairCV2', test.repairCV2);
    append('repairCV2Details', test.repairCV2Details);
    append('repairRV', test.repairRV);
    append('repairRVDetails', test.repairRVDetails);
    append('repairBC', test.repairBC);
    append('repairBCDetails', test.repairBCDetails);
    append('repairPvbAirInlet', test.repairPvbAirInlet);
    append('repairPvbAirInletDetails', test.repairPvbAirInletDetails);
    append('repairPvbCV', test.repairPvbCV);
    append('repairPvbCVDetails', test.repairPvbCVDetails);

    // Final test readings — main assembly
    append('finalCV1HeldPSID', test.finalCV1HeldPSID);
    append('finalCV1ClosedTight', test.finalCV1ClosedTight);
    append('finalCV2HeldPSID', test.finalCV2HeldPSID);
    append('finalCV2ClosedTight', test.finalCV2ClosedTight);
    append('finalRVOpenedPSID', test.finalRVOpenedPSID);
    append('finalBCHeldPSID', test.finalBCHeldPSID);
    append('finalBCClosedTight', test.finalBCClosedTight);
    append('finalPvbAirInletOpenedPSID', test.finalPvbAirInletOpenedPSID);
    append('finalPvbAirInletFullyOpened', test.finalPvbAirInletFullyOpened);
    append('finalPvbCVHeldPSID', test.finalPvbCVHeldPSID);

    // Initial test readings — bypass assembly
    append('initCV1HeldPSID2', test.initCV1HeldPSID2);
    append('initCV1ClosedTight2', test.initCV1ClosedTight2);
    append('initCV1Leaked2', test.initCV1Leaked2);
    append('initCV2HeldPSID2', test.initCV2HeldPSID2);
    append('initCV2ClosedTight2', test.initCV2ClosedTight2);
    append('initCV2Leaked2', test.initCV2Leaked2);
    append('initRVOpenedPSID2', test.initRVOpenedPSID2);
    append('initRVDidNotOpen2', test.initRVDidNotOpen2);

    // Repairs — bypass assembly
    append('repairCV12', test.repairCV12);
    append('repairCV1Details2', test.repairCV1Details2);
    append('repairCV22', test.repairCV22);
    append('repairCV2Details2', test.repairCV2Details2);
    append('repairRV2', test.repairRV2);
    append('repairRVDetails2', test.repairRVDetails2);

    // Final test readings — bypass assembly
    append('finalCV1HeldPSID2', test.finalCV1HeldPSID2);
    append('finalCV1ClosedTight2', test.finalCV1ClosedTight2);
    append('finalCV2HeldPSID2', test.finalCV2HeldPSID2);
    append('finalCV2ClosedTight2', test.finalCV2ClosedTight2);
    append('finalRVOpenedPSID2', test.finalRVOpenedPSID2);

    return fd;
}
