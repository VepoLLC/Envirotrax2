import { CommonModule, DatePipe, formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import {
    CalendarOptions,
    DatesSetInfo,
    DayCellInfo,
    DayHeaderInfo,
    EventClickInfo,
    EventInput,
    FullCalendarComponent,
    FullCalendarModule
} from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/angular/daygrid';
import classicThemePlugin from '@fullcalendar/angular/themes/classic';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { CompanyCalendarRange, CompanyRow } from '../../../../shared/models/crm/company';

// January to December, with the month index (0-11) of a Date as the id.
const MONTH_OPTIONS: InputOption[] = Array.from({ length: 12 }, (_, month) => ({
    id: String(month),
    text: formatDate(new Date(2000, month, 1), 'MMMM', 'en-US')
}));

// The Year list offers this many years before and after the year it was built for.
const YEAR_OPTIONS_AROUND = 10;

const SUNDAY = 0;
const SATURDAY = 6;

/**
 * V1 Vepo Manager's Contact Calendar: a month with each company on its next contact date, marked with its contact
 * type icon. The window that holds it loads the companies of the days shown, so the list under the calendar shares
 * them.
 */
@Component({
    selector: 'vp-company-calendar',
    templateUrl: './company-calendar.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FullCalendarModule
    ],
    providers: [DatePipe],
    styles: `
    .company-calendar-month {
        width: 10rem;
    }

    .company-calendar-year {
        width: 7rem;
    }
    `
})
export class CompanyCalendarComponent implements OnChanges {
    @ViewChild(FullCalendarComponent)
    private _calendar?: FullCalendarComponent;

    @Input()
    public companies?: CompanyRow[];

    // Emitted for the month the calendar opens on and for every month it moves to.
    @Output()
    public rangeChange: EventEmitter<CompanyCalendarRange> = new EventEmitter();

    @Output()
    public companySelect: EventEmitter<CompanyRow> = new EventEmitter();

    public events: EventInput[] = [];

    public readonly monthOptions: InputOption[] = MONTH_OPTIONS;
    public yearOptions: InputOption[] = [];

    // The select options carry string ids, so the selects bind these.
    public monthId: string = '';
    public yearId: string = '';

    public readonly calendarOptions: CalendarOptions = {
        plugins: [classicThemePlugin, dayGridPlugin],
        initialView: 'dayGridMonth',

        // V1's arrows and Month and Year lists, above the calendar, take the place of FullCalendar's toolbar.
        headerToolbar: false,

        className: 'vp-calendar',
        dayHeaderFormat: { weekday: 'long' },
        dayHeaderClass: info => this.getDayHeaderClass(info),
        dayCellClass: info => this.getDayCellClass(info),

        // The same height at full width and beside a details window: the whole month fits on the screen, with two
        // lines a day. A day with more companies than fit shows "+N more", which opens the day's whole list.
        height: 560,
        dayMaxEvents: true,

        // A line per company, marked with its contact type icon instead of FullCalendar's colored dot.
        eventDisplay: 'list-item',
        listItemEventBeforeClass: 'd-none',
        eventClass: 'vp-calendar-event',

        datesSet: info => this.onDatesSet(info),
        eventClick: info => this.onEventClick(info)
    };

    private _companiesById = new Map<string, CompanyRow>();

    constructor(private readonly _datePipe: DatePipe) {
        // FullCalendar opens on the current month.
        this.setMonth(new Date());
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['companies']) {
            const companies = (this.companies ?? []).filter(company => company.nextContactDate);

            this._companiesById = new Map(companies.map(company => [String(company.id), company]));
            this.events = companies.map(company => this.toEvent(company));
        }
    }

    public previousMonth(): void {
        this._calendar?.getApi().prev();
    }

    public nextMonth(): void {
        this._calendar?.getApi().next();
    }

    public onMonthChange(monthId: string): void {
        this.goToMonth(Number(this.yearId), Number(monthId));
    }

    public onYearChange(yearId: string): void {
        this.goToMonth(Number(yearId), Number(this.monthId));
    }

    private goToMonth(year: number, month: number): void {
        this._calendar?.getApi().gotoDate(new Date(year, month, 1));
    }

    private async onDatesSet(info: DatesSetInfo): Promise<void> {
        // FullCalendar reports the month it opens on while it renders, inside Angular's change detection. Waiting a
        // microtask moves the Month and Year lists, and the reload this starts in the window, after that check.
        await Promise.resolve();

        // currentStart is the first day of the month. start and end also take in the days shown from the months
        // before and after it.
        this.setMonth(info.view.currentStart);

        this.rangeChange.emit({
            startDate: this._datePipe.transform(info.start, 'yyyy-MM-dd')!,
            endDate: this._datePipe.transform(info.end, 'yyyy-MM-dd')!
        });
    }

    private onEventClick(info: EventClickInfo): void {
        const company = this._companiesById.get(info.event.id);

        if (company) {
            this.companySelect.emit(company);
        }
    }

    private setMonth(date: Date): void {
        this.monthId = String(date.getMonth());
        this.yearId = String(date.getFullYear());

        // Rebuilt only when the arrows go past the listed years, so the list stays the same while it is used.
        if (!this.yearOptions.some(option => option.id === this.yearId)) {
            this.yearOptions = this.buildYearOptions(date.getFullYear());
        }
    }

    private buildYearOptions(year: number): InputOption[] {
        const yearOptions: InputOption[] = [];

        for (let optionYear = year - YEAR_OPTIONS_AROUND; optionYear <= year + YEAR_OPTIONS_AROUND; optionYear++) {
            yearOptions.push({ id: String(optionYear), text: String(optionYear) });
        }

        return yearOptions;
    }

    // V1's weekday headers are blue and its weekend headers grey. The "+N more" list's header is always blue.
    private getDayHeaderClass(info: DayHeaderInfo): string {
        return this.isWeekend(info.dow) && !info.inPopover ? 'vp-calendar-weekend-header' : 'vp-calendar-day-header';
    }

    // V1 shades the weekends and greys out the days of the months before and after.
    private getDayCellClass(info: DayCellInfo): string {
        if (info.isOther) {
            return 'vp-calendar-other-month';
        }

        return this.isWeekend(info.dow) ? 'vp-calendar-weekend' : '';
    }

    private isWeekend(dayOfWeek: number): boolean {
        return dayOfWeek === SUNDAY || dayOfWeek === SATURDAY;
    }

    private toEvent(company: CompanyRow): EventInput {
        return {
            id: String(company.id),
            title: company.name,

            // V1 saved a time with some next contact dates. The calendar places the company on the day.
            start: this._datePipe.transform(company.nextContactDate, 'yyyy-MM-dd')!,
            allDay: true,

            extendedProps: {
                iconClass: company.contactTypeIconClass,
                tooltip: `${company.name} - ${company.contactTypeLabel}`
            }
        };
    }
}
