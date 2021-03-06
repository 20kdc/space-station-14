﻿using System;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Alert
{
    /// <summary>
    /// An alert popup with associated icon, tooltip, and other data.
    /// </summary>
    [Prototype("alert")]
    public class AlertPrototype : IPrototype
    {
        /// <summary>
        /// Type of alert, no 2 alert prototypes should have the same one.
        /// </summary>
        public AlertType AlertType { get; private set; }

        /// <summary>
        /// Path to the icon (png) to show in alert bar. If severity levels are supported,
        /// this should be the path to the icon without the severity number
        /// (i.e. hot.png if there is hot1.png and hot2.png). Use <see cref="GetIconPath"/>
        /// to get the correct icon path for a particular severity level.
        /// </summary>
        [ViewVariables]
        public string IconPath { get; private set; }

        /// <summary>
        /// Name to show in tooltip window. Accepts formatting.
        /// </summary>
        public FormattedMessage Name { get; private set; }

        /// <summary>
        /// Description to show in tooltip window. Accepts formatting.
        /// </summary>
        public FormattedMessage Description { get; private set; }

        /// <summary>
        /// Category the alert belongs to. Only one alert of a given category
        /// can be shown at a time. If one is shown while another is already being shown,
        /// it will be replaced. This can be useful for categories of alerts which should naturally
        /// replace each other and are mutually exclusive, for example lowpressure / highpressure,
        /// hot / cold. If left unspecified, the alert will not replace or be replaced by any other alerts.
        /// </summary>
        public AlertCategory? Category { get; private set; }

        /// <summary>
        /// Key which is unique w.r.t category semantics (alerts with same category have equal keys,
        /// alerts with no category have different keys).
        /// </summary>
        public AlertKey AlertKey { get; private set; }

        /// <summary>
        /// -1 (no effect) unless MaxSeverity is specified. Defaults to 1. Minimum severity level supported by this state.
        /// </summary>
        public short MinSeverity => MaxSeverity == -1 ? (short) -1 : _minSeverity;
        private short _minSeverity;

        /// <summary>
        /// Maximum severity level supported by this state. -1 (default) indicates
        /// no severity levels are supported by the state.
        /// </summary>
        public short MaxSeverity { get; private set; }

        /// <summary>
        /// Indicates whether this state support severity levels
        /// </summary>
        public bool SupportsSeverity => MaxSeverity != -1;

        public void LoadFrom(YamlMappingNode mapping)
        {
            var serializer = YamlObjectSerializer.NewReader(mapping);

            serializer.DataField(this, x => x.IconPath, "icon", string.Empty);
            serializer.DataField(this, x => x.MaxSeverity, "maxSeverity", (short) -1);
            serializer.DataField(ref _minSeverity, "minSeverity", (short) 1);

            serializer.DataReadFunction("name", string.Empty,
                s => Name = FormattedMessage.FromMarkup(s));
            serializer.DataReadFunction("description", string.Empty,
                s => Description = FormattedMessage.FromMarkup(s));

            serializer.DataField(this, x => x.AlertType, "alertType", AlertType.Error);
            if (AlertType == AlertType.Error)
            {
                Logger.ErrorS("alert", "missing or invalid alertType for alert with name {0}", Name);
            }

            if (serializer.TryReadDataField("category", out AlertCategory alertCategory))
            {
                Category = alertCategory;
            }
            AlertKey = new AlertKey(AlertType, Category);
        }

        /// <param name="severity">severity level, if supported by this alert</param>
        /// <returns>the icon path to the texture for the provided severity level</returns>
        public string GetIconPath(short? severity = null)
        {
            if (!SupportsSeverity && severity != null)
            {
                Logger.WarningS("alert", "attempted to get icon path for severity level for alert {0}, but" +
                                          " this alert does not support severity levels", AlertType);
            }
            if (!SupportsSeverity) return IconPath;
            if (severity == null)
            {
                Logger.WarningS("alert", "attempted to get icon path without severity level for alert {0}," +
                                " but this alert requires a severity level. Using lowest" +
                                " valid severity level instead...", AlertType);
                severity = MinSeverity;
            }

            if (severity < MinSeverity)
            {
                Logger.WarningS("alert", "attempted to get icon path with severity level {0} for alert {1}," +
                                          " but the minimum severity level for this alert is {2}. Using" +
                                          " lowest valid severity level instead...", severity, AlertType, MinSeverity);
                severity = MinSeverity;
            }
            if (severity > MaxSeverity)
            {
                Logger.WarningS("alert", "attempted to get icon path with severity level {0} for alert {1}," +
                                          " but the max severity level for this alert is {2}. Using" +
                                          " highest valid severity level instead...", severity, AlertType, MaxSeverity);
                severity = MaxSeverity;
            }

            // split and add the severity number to the path
            var ext = IconPath.LastIndexOf('.');
            return IconPath.Substring(0, ext) + severity + IconPath.Substring(ext, IconPath.Length - ext);
        }
    }

    /// <summary>
    /// Key for an alert which is unique (for equality and hashcode purposes) w.r.t category semantics.
    /// I.e., entirely defined by the category, if a category was specified, otherwise
    /// falls back to the id.
    /// </summary>
    [Serializable, NetSerializable]
    public struct AlertKey
    {
        private readonly AlertType? _alertType;
        private readonly AlertCategory? _alertCategory;

        /// NOTE: if the alert has a category you must pass the category for this to work
        /// properly as a key. I.e. if the alert has a category and you pass only the ID, and you
        /// compare this to another AlertKey that has both the category and the same ID, it will not consider them equal.
        public AlertKey(AlertType? alertType, AlertCategory? alertCategory)
        {
            // if there is a category, ignore the alerttype.
            if (alertCategory != null)
            {
                _alertCategory = alertCategory;
                _alertType = null;
            }
            else
            {
                _alertCategory = null;
                _alertType = alertType;
            }
        }

        public bool Equals(AlertKey other)
        {
            return _alertType == other._alertType && _alertCategory == other._alertCategory;
        }

        public override bool Equals(object obj)
        {
            return obj is AlertKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_alertType, _alertCategory);
        }

        /// <param name="category">alert category, must not be null</param>
        /// <returns>An alert key for the provided alert category</returns>
        public static AlertKey ForCategory(AlertCategory category)
        {
            return new AlertKey(null, category);
        }
    }
}
