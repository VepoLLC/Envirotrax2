import { ChangeDetectorRef, Component, EventEmitter, NgZone, OnInit, Output } from "@angular/core";
import { AuthorizeNetService } from "../../services/authorize-net/authorize-net.service";

export interface CreditCardToken {
    dataDescriptor: string;
    dataValue: string;
}

interface AcceptUiResponse {
    messages: {
        resultCode: string;
        message: { code: string; text: string }[];
    };
    opaqueData: {
        dataDescriptor: string;
        dataValue: string;
    };
}

declare global {
    interface Window {
        acceptUiResponseHandler?: (response: AcceptUiResponse) => void;
    }
}

@Component({
    selector: 'app-credit-card-payment',
    standalone: false,
    templateUrl: './credit-card-payment.component.html'
})
export class CreditCardPaymentComponent implements OnInit {
    @Output()
    public tokenCaptured = new EventEmitter<CreditCardToken>();

    public isReady: boolean = false;
    public hasCapturedToken: boolean = false;
    public errors: string[] = [];

    public apiLoginId: string = '';
    public publicClientKey: string = '';

    constructor(
        private readonly _authorizeNetService: AuthorizeNetService,
        private readonly _ngZone: NgZone,
        private readonly _changeDetectorRef: ChangeDetectorRef
    ) { }

    public async ngOnInit(): Promise<void> {
        const clientConfig = await this._authorizeNetService.getClientConfig();

        this.apiLoginId = clientConfig.apiLoginId;
        this.publicClientKey = clientConfig.publicClientKey;

        // Force the data-apiLoginID/data-clientKey attributes onto the DOM now, before AcceptUI.js
        // loads and scans the page for `.AcceptUI` buttons — otherwise it can capture the button
        // while those attributes are still empty (script load can outrace our config fetch).
        this._changeDetectorRef.detectChanges();

        window.acceptUiResponseHandler = (response) => this._ngZone.run(() => this.handleResponse(response));

        await this._authorizeNetService.ensureAcceptUiScriptLoaded();

        this.isReady = true;
    }

    public reset(): void {
        this.hasCapturedToken = false;
        this.errors = [];
    }

    public onEnterCardDetailsClick(): void {
        // Clear the previous attempt's result so a repeat "Enter Card Details" click doesn't keep
        // showing a stale success/error message from an earlier, already-used token.
        this.reset();
    }

    private handleResponse(response: AcceptUiResponse): void {
        this.errors = [];

        if (response.messages.resultCode === 'Error') {
            this.errors = response.messages.message.map(m => m.text);
            this.hasCapturedToken = false;
        } else {
            this.hasCapturedToken = true;

            this.tokenCaptured.emit({
                dataDescriptor: response.opaqueData.dataDescriptor,
                dataValue: response.opaqueData.dataValue
            });
        }
    }
}
