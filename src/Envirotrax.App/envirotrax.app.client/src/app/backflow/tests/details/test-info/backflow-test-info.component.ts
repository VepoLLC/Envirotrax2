import { Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestOptionsService } from "../../../../shared/services/backflow/backflow-test-options.service";
import { BackflowDeviceType, BackflowReasonForTest, BackflowTestResult } from "../../../../shared/models/backflow/backflow-test-enums";

interface ValveCheck {
    label: string;
    field: keyof BackflowTest;
}

interface ValveColumn {
    index: number;
    group: 'main' | 'bypass';
    header: string;
    initialVerb: string;
    initialValue?: keyof BackflowTest;
    initialChecks: ValveCheck[];
    repairField?: keyof BackflowTest;
    repairDetailsField?: keyof BackflowTest;
    finalVerb?: string;
    finalValue?: keyof BackflowTest;
    finalChecks: ValveCheck[];
}

@Component({
    selector: 'vp-backflow-test-info',
    standalone: false,
    templateUrl: './backflow-test-info.component.html'
})
export class BackflowTestInfoComponent implements OnChanges {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;

    @Output() public save = new EventEmitter<NgForm>();

    public gaugeDescription: string = '';
    public visibleValveColumns: ValveColumn[] = [];
    public showAssemblySubheader: boolean = false;
    public mainColumnCount: number = 0;
    public bypassColumnCount: number = 0;
    public showMeterTest: boolean = false;

    public readonly BackflowReasonForTest = BackflowReasonForTest;
    public readonly BackflowDeviceType = BackflowDeviceType;

    private readonly _bypassDeviceTypes: string[] = [BackflowDeviceType.DCD, BackflowDeviceType.DCD2, BackflowDeviceType.RPPD, BackflowDeviceType.RPPD2];
    private readonly _meterTestDeviceTypes: string[] = [BackflowDeviceType.DCD, BackflowDeviceType.DCD2, BackflowDeviceType.RPPD, BackflowDeviceType.RPPD2];

    private readonly _valveColumns: ValveColumn[] = [
        {
            index: 1, group: 'main', header: 'Check Valve #1',
            initialVerb: 'Held at', initialValue: 'initCV1HeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV1ClosedTight' }, { label: 'Leaked', field: 'initCV1Leaked' }],
            repairField: 'repairCV1', repairDetailsField: 'repairCV1Details',
            finalVerb: 'Held at', finalValue: 'finalCV1HeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV1ClosedTight' }]
        },
        {
            index: 2, group: 'main', header: 'Check Valve #2 ***',
            initialVerb: 'Held at', initialValue: 'initCV2HeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV2ClosedTight' }, { label: 'Leaked', field: 'initCV2Leaked' }],
            repairField: 'repairCV2', repairDetailsField: 'repairCV2Details',
            finalVerb: 'Held at', finalValue: 'finalCV2HeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV2ClosedTight' }]
        },
        {
            index: 3, group: 'main', header: 'Relief Valve',
            initialVerb: 'Opened at', initialValue: 'initRVOpenedPSID',
            initialChecks: [{ label: 'Did not open', field: 'initRVDidNotOpen' }],
            repairField: 'repairRV', repairDetailsField: 'repairRVDetails',
            finalVerb: 'Opened at', finalValue: 'finalRVOpenedPSID',
            finalChecks: []
        },
        {
            index: 4, group: 'bypass', header: 'Check Valve #1',
            initialVerb: 'Held at', initialValue: 'initCV1HeldPSID2',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV1ClosedTight2' }, { label: 'Leaked', field: 'initCV1Leaked2' }],
            repairField: 'repairCV12', repairDetailsField: 'repairCV1Details2',
            finalVerb: 'Held at', finalValue: 'finalCV1HeldPSID2',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV1ClosedTight2' }]
        },
        {
            index: 5, group: 'bypass', header: 'Check Valve #2 ***',
            initialVerb: 'Held at', initialValue: 'initCV2HeldPSID2',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV2ClosedTight2' }, { label: 'Leaked', field: 'initCV2Leaked2' }],
            repairField: 'repairCV22', repairDetailsField: 'repairCV2Details2',
            finalVerb: 'Held at', finalValue: undefined,
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV2ClosedTight2' }]
        },
        {
            index: 6, group: 'bypass', header: 'Relief Valve',
            initialVerb: 'Opened at', initialValue: 'initRVOpenedPSID2',
            initialChecks: [{ label: 'Did not open', field: 'initRVDidNotOpen2' }],
            repairField: 'repairRV2', repairDetailsField: 'repairRVDetails2',
            finalVerb: 'Opened at', finalValue: 'finalRVOpenedPSID2',
            finalChecks: []
        },
        {
            index: 7, group: 'bypass', header: 'Bypass Check',
            initialVerb: 'Held at', initialValue: 'initBCHeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initBCClosedTight' }, { label: 'Leaked', field: 'initBCLeaked' }],
            repairField: 'repairBC', repairDetailsField: 'repairBCDetails',
            finalVerb: 'Held at', finalValue: 'finalBCHeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalBCClosedTight' }]
        },
        {
            index: 8, group: 'main', header: 'Air Inlet',
            initialVerb: 'Opened at', initialValue: 'initPvbAirInletOpenedPSID',
            initialChecks: [{ label: 'Did not open', field: 'initPvbAirInletDidNotOpen' }, { label: 'Did it fully open', field: 'initPvbAirInletFullyOpened' }],
            finalVerb: 'Opened at', finalValue: 'finalPvbAirInletOpenedPSID',
            finalChecks: [{ label: 'Did it fully open', field: 'finalPvbAirInletFullyOpened' }]
        },
        {
            index: 9, group: 'main', header: 'Check Valve',
            initialVerb: 'Held at', initialValue: 'initPvbCVHeldPSID',
            initialChecks: [{ label: 'Leaked', field: 'initPvbCVLeaked' }],
            finalVerb: 'Held at', finalValue: 'finalPvbCVHeldPSID',
            finalChecks: []
        }
    ];

    private readonly _deviceColumnMap: Record<string, number[]> = {
        [BackflowDeviceType.DC]: [1, 2],
        [BackflowDeviceType.DCD]: [1, 2, 4, 5],
        [BackflowDeviceType.DCD2]: [1, 2, 7],
        [BackflowDeviceType.RP]: [1, 2, 3],
        [BackflowDeviceType.RPPD]: [1, 2, 3, 4, 5, 6],
        [BackflowDeviceType.RPPD2]: [1, 2, 3, 7],
        [BackflowDeviceType.PVB]: [8, 9],
        [BackflowDeviceType.SVB]: [8, 9]
    };

    private readonly _testResultLabels: Record<number, string> = {
        [BackflowTestResult.Pass]: 'Passed',
        [BackflowTestResult.Fail]: 'Failed',
        [BackflowTestResult.PassAfterRepairs]: 'Passed After Repairs'
    };

    constructor(private readonly _optionsService: BackflowTestOptionsService) { }

    public ngOnChanges(): void {
        this.updateDisplayState();
    }

    public getReasonForTestLabel(): string {
        if (this.test?.reasonForTest == null) {
            return '';
        }
        return this._optionsService.reasonOptions.find(o => o.id === this.test!.reasonForTest)?.text ?? '';
    }

    public getTestResultLabel(): string {
        if (this.test?.testResult == null) {
            return '';
        }
        return this._testResultLabels[this.test.testResult] ?? '';
    }

    public getDeviceTypeLabel(): string {
        if (!this.test?.deviceType) {
            return '';
        }
        return this._optionsService.deviceTypeOptions.find(o => o.id === this.test!.deviceType)?.text ?? this.test.deviceType;
    }

    public field(name: keyof BackflowTest | undefined): unknown {
        if (!name || this.test == null) {
            return null;
        }
        return this.test[name];
    }

    private updateDisplayState(): void {
        const test = this.test;

        if (test == null) {
            this.gaugeDescription = '';
            this.visibleValveColumns = [];
            this.showAssemblySubheader = false;
            this.mainColumnCount = 0;
            this.bypassColumnCount = 0;
            this.showMeterTest = false;
            return;
        }

        const gaugeBase = [test.gaugeManufacturer, test.gaugeModel].filter(Boolean).join(' ');
        const gaugeSuffix = test.gaugeNonPotable ? ' (non-potable)' : ' (potable)';
        this.gaugeDescription = gaugeBase ? gaugeBase + gaugeSuffix : '';

        const indices = (test.deviceType && this._deviceColumnMap[test.deviceType]) || [];
        this.visibleValveColumns = this._valveColumns.filter(c => indices.includes(c.index));

        this.showAssemblySubheader = !!test.deviceType && this._bypassDeviceTypes.includes(test.deviceType);
        this.showMeterTest = !!test.deviceType && this._meterTestDeviceTypes.includes(test.deviceType);

        this.mainColumnCount = this.visibleValveColumns.filter(c => c.group === 'main').length;
        this.bypassColumnCount = this.visibleValveColumns.filter(c => c.group === 'bypass').length;
    }
}
