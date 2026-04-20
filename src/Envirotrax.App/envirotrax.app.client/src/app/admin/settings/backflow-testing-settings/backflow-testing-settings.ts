import { BackflowExpiredType, BackflowExpiringType, BackflowNonCompliantType, BackflowOutOfServiceType, BackflowTestingMethodType } from '../../../shared/models/settings/backflow-testing-settings-enum';
import { BackflowSettings } from '../../../shared/models/settings/backflow-settings';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastService } from '../../../shared/services/toast.service';
import { HelperService } from '../../../shared/services/helpers/helper.service';
import { BackflowTestingSettingsService } from '../../../shared/services/settings/backflow-testing-settings.service';

@Component({
  selector: 'app-backflow-testing-settings',
  standalone: false,
  templateUrl: './backflow-testing-settings.html',
})
export class BackflowTestingSettings implements OnInit {

    public settings: BackflowSettings = {
        testingMethod: BackflowTestingMethodType.USC, // Default to USC
        gracePeriodDays: null,
        outOfServiceType: BackflowOutOfServiceType.VepoManaged, // Default to Vepo Managed
        expiringNotice1: BackflowExpiringType.Off,
        expiringNotice2: BackflowExpiringType.Off,
        expiredNotice1: BackflowExpiredType.Off,
        expiredNotice2: BackflowExpiredType.Off,
        backflowNonCompliant1: BackflowNonCompliantType.Off,
        backflowNonCompliant2: BackflowNonCompliantType.Off, 
        expiringLettersBackgroundColor: '#d3d3d3',
        expiringLettersForegroundColor: '#000000',
        expiringLettersBorderColor: '#000000',
        expiredLettersBackgroundColor: '#d3d3d3',
        expiredLettersForegroundColor: '#000000',
        expiredLettersBorderColor: '#000000',
        nonCompliantLettersBackgroundColor: '#d3d3d3',
        nonCompliantLettersForegroundColor: '#000000',
        nonCompliantLettersBorderColor: '#000000',  
    };

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

      public readonly testingMethodOptions = [
          { id: BackflowTestingMethodType.USC, text: 'USC' },
          { id: BackflowTestingMethodType.ASSE, text: 'ASSE' },
          { id: BackflowTestingMethodType.TREEO, text: 'TREEO' }
      ];
  
      public readonly outOfServiceOptions = [
          { id: BackflowOutOfServiceType.VepoManaged, text: 'Vepo Managed' },
          { id: BackflowOutOfServiceType.WaterSupplierManaged, text: 'Water Supplier Managed' }
      ];
  
      public readonly expiringNoticeOptions = [
          { id: BackflowExpiringType.Off, text: 'Off' },
          { id: BackflowExpiringType.TwoMonths, text: 'Expiring in 2 Months' },
          { id: BackflowExpiringType.NextMonth, text: 'Expiring Next Month' },
          { id: BackflowExpiringType.ThisMonth, text: 'Expiring This Month' },
      ];
  
      public readonly expiredNoticeOptions = [
          { id: BackflowExpiredType.Off, text: 'Off' },
          { id: BackflowExpiredType.LastMonth, text: 'Expired Last Month' },
          { id: BackflowExpiredType.TwoMonthsAgo, text: 'Expired 2 Months Ago' },
          { id: BackflowExpiredType.ThreeMonthsAgo, text: 'Expired 3 Months Ago' },
          { id: BackflowExpiredType.ThisMonth, text: 'Expired This Month' },
      ];
  
      public readonly backflowNonCompliantOptions = [
          { id: BackflowNonCompliantType.Off, text: 'Off' },
          { id: BackflowNonCompliantType.ThirtyPlusDays, text: 'Expired 30+ Days' },
          { id: BackflowNonCompliantType.SixtyPlusDays, text: 'Expired 60+ Days' },
          { id: BackflowNonCompliantType.NinetyPlusDays, text: 'Expired 90+ Days' },
          { id: BackflowNonCompliantType.OneTwentyPlusDays, text: 'Expired 120+ Days' },
          { id: BackflowNonCompliantType.OneFiftyPlusDays, text: 'Expired 150+ Days' },
          { id: BackflowNonCompliantType.OneEightyPlusDays, text: 'Expired 180+ Days' },
          { id: BackflowNonCompliantType.PastDueLastMonth, text: 'Expired Last Month' },
          { id: BackflowNonCompliantType.ExpiredTwoMonthsAgo, text: 'Expired 2 Months Ago' },
      ];
    
  
  constructor(
    private readonly _backflowSettingsService: BackflowTestingSettingsService,
    private readonly _helper: HelperService,
    private readonly _toastService: ToastService,
  ) {}

  public async ngOnInit(): Promise<void> {
      await this.getSettings();
  }

  private async getSettings(): Promise<void> {
      this.isLoading = true;

      try {
          this.settings = await this._backflowSettingsService.get();
      } finally {
          this.isLoading = false;
      }
  }

  public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                console.log('Saving settings:', this.settings);

                let result;
                if (this.settings.id) {
                    //result = await this._backflowSettingsService.update(this.settings);
                } else {
                    //result = await this._backflowSettingsService.add(this.settings);
                }

                if (result) {
                    this.settings = result;
                }

                this._toastService.successfullySaved('Backflow Testing Settings');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Backflow Testing Settings');

            } finally {
                this.isLoading = false;
            }
        }
    }

}
