import { Chart, LegendItem } from 'chart.js';

// Chart.js draws axis ticks, grid lines and legend text on the canvas, which CSS can't theme
// directly. These helpers read the theme-driven CSS custom properties (defined in styles.css and
// overridden in dark-theme.css) so canvas colors follow the active light/dark theme. They're used
// as Chart.js scriptable options / generateLabels, which re-evaluate on every chart.update().

export function readCssVar(name: string, fallback: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim() || fallback;
}

// Axis tick labels + active legend labels.
export function chartTickColor(): string {
    return readCssVar('--bf-chart-tick-color', '#495057');
}

// Grid lines.
export function chartGridColor(): string {
    return readCssVar('--bf-chart-grid-color', '#dee2e6');
}

// Toggled-off (disabled) legend labels.
export function chartMutedColor(): string {
    return readCssVar('--bf-chart-muted-color', '#adb5bd');
}

// Theme-aware legend labels: active labels use the tick color, toggled-off ones the muted color.
// (Also clears `hidden` so Chart.js doesn't draw a strike-through over the greyed label.)
export function themeLegendLabels(chart: Chart): LegendItem[] {
    const items = Chart.defaults.plugins.legend.labels.generateLabels(chart);

    items.forEach(item => {
        item.fontColor = item.hidden ? chartMutedColor() : chartTickColor();
        item.hidden = false;
    });

    return items;
}

// Re-runs `update` whenever the app's light/dark theme toggles (the `vp-dark-theme` class on <body>),
// so canvas charts repaint their theme-driven colors immediately instead of on the next hover/refresh.
// The repaint is deferred to the next animation frame and coalesced. Returns a disposer to call on
// component destroy.
export function onThemeChange(update: () => void): () => void {
    let frame: number | undefined;

    const observer = new MutationObserver(() => {
        if (frame != null) {
            cancelAnimationFrame(frame);
        }

        frame = requestAnimationFrame(update);
    });

    observer.observe(document.body, { attributes: true, attributeFilter: ['class'] });

    return () => {
        observer.disconnect();

        if (frame != null) {
            cancelAnimationFrame(frame);
        }
    };
}
