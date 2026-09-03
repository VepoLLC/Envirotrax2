import { BackflowDeviceType } from '../../../shared/models/backflow/backflow-test';

export interface BackflowAssemblyVisibility {
    hasBypassAssembly: boolean;
    showMainCv1: boolean;
    showMainCv2: boolean;
    showMainRv: boolean;
    showBypassCv1: boolean;
    showBypassCv2: boolean;
    showBypassRv: boolean;
    showBypassCheck: boolean;
    showPvb: boolean;
    showMeterTest: boolean;
    showAirGap: boolean;
    showGauge: boolean;
    showInitialAndFinal: boolean;
}

export function buildAssemblyVisibility(deviceType?: string): BackflowAssemblyVisibility {
    const isAirGap = deviceType === BackflowDeviceType.AG;
    const isPvb = deviceType === BackflowDeviceType.PVB || deviceType === BackflowDeviceType.SVB;

    const hasBypassAssembly = deviceType === BackflowDeviceType.DCD
        || deviceType === BackflowDeviceType.DCD2
        || deviceType === BackflowDeviceType.RPPD
        || deviceType === BackflowDeviceType.RPPD2;

    const hasReliefValve = deviceType === BackflowDeviceType.RP
        || deviceType === BackflowDeviceType.RPPD
        || deviceType === BackflowDeviceType.RPPD2;

    const hasBypassCheckOnly = deviceType === BackflowDeviceType.DCD2
        || deviceType === BackflowDeviceType.RPPD2;

    const hasBypassValves = deviceType === BackflowDeviceType.DCD
        || deviceType === BackflowDeviceType.RPPD;

    const showMainValves = !isAirGap && !isPvb;

    return {
        hasBypassAssembly: hasBypassAssembly,
        showMainCv1: showMainValves,
        showMainCv2: showMainValves,
        showMainRv: showMainValves && hasReliefValve,
        showBypassCv1: hasBypassValves,
        showBypassCv2: hasBypassValves,
        showBypassRv: deviceType === BackflowDeviceType.RPPD,
        showBypassCheck: hasBypassCheckOnly,
        showPvb: isPvb,
        showMeterTest: hasBypassAssembly,
        showAirGap: isAirGap,
        showGauge: !isAirGap,
        showInitialAndFinal: !isAirGap
    };
}
