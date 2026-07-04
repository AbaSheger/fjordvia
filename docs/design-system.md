# Fjordvia Design System

Fjordvia is a Nordic enterprise SaaS brand for integrations, operations, and back-office workflow visibility. The design system below defines a premium, calm, and highly functional visual language for the product UI.

The system is inspired generally by modern enterprise SaaS products with polished spacing, crisp typography, clear hierarchy, and restrained color use. It does not copy any brand, layout, logo, text, or proprietary asset.

## Brand Principles

- Calm and precise.
- Dense, but never cramped.
- Functional first, decorative second.
- Trustworthy color with limited accents.
- Clear states for success, warning, failure, and pending work.
- Enterprise clarity over marketing flourish.

## Color System

Use the following CSS variables as the base palette.

```css
:root {
  --fjordvia-bg: #eef3f5;
  --fjordvia-surface: #ffffff;
  --fjordvia-surface-muted: #f7fafb;
  --fjordvia-border: #d8e1e5;
  --fjordvia-border-strong: #b9c9d1;
  --fjordvia-text: #17252f;
  --fjordvia-text-muted: #5d7480;
  --fjordvia-heading: #0f1d26;
  --fjordvia-primary: #1f6f8b;
  --fjordvia-primary-hover: #195e76;
  --fjordvia-primary-soft: #d9eef4;
  --fjordvia-accent: #244252;
  --fjordvia-accent-soft: #e6edf0;
  --fjordvia-success: #1d6b3a;
  --fjordvia-success-soft: #edf8f1;
  --fjordvia-warning: #84600d;
  --fjordvia-warning-soft: #fff6df;
  --fjordvia-danger: #9f2d2d;
  --fjordvia-danger-soft: #fff1f1;
  --fjordvia-info: #185e8a;
  --fjordvia-info-soft: #eaf4fb;
  --fjordvia-shadow: 0 10px 24px rgba(23, 37, 47, 0.06);
}
```

Color usage guidance:

- Use `--fjordvia-primary` for primary actions and key highlights.
- Use `--fjordvia-accent` for secondary emphasis and structural contrast.
- Use success, warning, and danger colors only for status and alerts.
- Keep page backgrounds quiet. Reserve strong color for controls, badges, and states.
- Avoid gradient-heavy UI unless the surface is a hero or landing area.

## Typography

Use a modern sans-serif stack with a Nordic enterprise feel. Prioritize readability and a compact but generous rhythm.

Recommended stack:

```css
:root {
  --fjordvia-font-sans: "Inter", "Segoe UI", "Helvetica Neue", Arial, sans-serif;
  --fjordvia-font-mono: "SFMono-Regular", "Consolas", "Liberation Mono", monospace;
}
```

Type scale:

- Page title: `32px`, weight `700`, line-height `1.15`
- Section title: `18px`, weight `700`, line-height `1.25`
- Body: `14px` to `16px`, weight `400` or `500`, line-height `1.5`
- Helper text: `12px` to `13px`, weight `400` or `600`, line-height `1.4`

Typography rules:

- Use sentence case for UI text.
- Keep letter spacing at `0`.
- Use monospaced text only for IDs, references, and structured values.
- Avoid oversized display type except for a true top-level landing hero.

## Spacing

Use an 8px spacing system.

```css
:root {
  --fjordvia-space-1: 4px;
  --fjordvia-space-2: 8px;
  --fjordvia-space-3: 12px;
  --fjordvia-space-4: 16px;
  --fjordvia-space-5: 20px;
  --fjordvia-space-6: 24px;
  --fjordvia-space-8: 32px;
  --fjordvia-space-10: 40px;
  --fjordvia-space-12: 48px;
}
```

Spacing guidance:

- Use `16px` and `24px` as the most common layout gaps.
- Use `8px` for inline control spacing and compact metadata clusters.
- Use `32px` and `48px` for section separation.
- Keep component internals consistent with the same spacing scale.

## App Shell

The app shell should feel like a focused operations console.

Layout rules:

- Use a centered content width with a comfortable max width, typically `1120px` to `1200px`.
- Keep a subtle background and distinct surfaces for content blocks.
- Use a sticky or fixed header only if the workflow benefits from persistent navigation.
- Reserve the first screen for the dashboard, not a marketing intro.
- Page sections should be full-width bands with constrained inner content, not nested floating cards.

Shell tokens:

```css
:root {
  --fjordvia-shell-max-width: 1180px;
  --fjordvia-shell-gutter: 16px;
  --fjordvia-shell-radius: 8px;
}
```

## Cards

Cards are for compact repeated items, metric summaries, modals, and framed tools.

Card rules:

- Border radius: `8px` or less.
- Border: subtle and visible.
- Shadow: restrained.
- Title and content should be clearly separated.
- Avoid placing cards inside larger cards unless the nested surface is a true repeated item.

Card structure:

- Header for title and actions.
- Body for content.
- Footer only when the action set is distinct.

## Tables

Tables are a primary enterprise pattern in Fjordvia.

Table rules:

- Use fixed, predictable column headers.
- Keep alignment consistent by data type.
- Use compact row height with enough breathing room for scanning.
- Right-align actions.
- Provide horizontal scrolling on smaller viewports instead of collapsing important data.

Table styling guidance:

- Header text: uppercase, small, muted, high-contrast enough to scan.
- Row borders: light and consistent.
- Hover state: subtle surface tint, not a strong color wash.
- Numeric values: align right when comparison matters.

## Forms

Forms should feel operational and fast to scan.

Form rules:

- Label every field.
- Prefer one clear primary action.
- Group related fields in rows on larger screens.
- Use helper text only when it prevents errors or removes ambiguity.
- Keep validation messages short and specific.

Input styling guidance:

- Border radius: `6px` to `8px`.
- Use a clear focus ring.
- Prefer simple, native controls where possible.
- Keep disabled states readable, not faded to the point of illegibility.

Recommended field hierarchy:

- Selects for partner and workflow choice.
- Text inputs for identifiers.
- Numeric inputs for quantity and value.
- Textarea only when the content truly needs it.

## Buttons

Button hierarchy:

- Primary button: main form submission or key workflow action.
- Secondary button: refresh, cancel, or lower-priority actions.
- Tertiary button: minimal actions in dense layouts.
- Destructive button: delete, remove, or irreversible actions.

Button rules:

- Use concise labels.
- Keep width stable enough to avoid layout shift.
- Pair icon and text only when the icon adds meaning.
- Use a disabled state for in-flight or unavailable actions.

Recommended button tokens:

```css
:root {
  --fjordvia-button-radius: 6px;
  --fjordvia-button-height: 38px;
}
```

## Status Badges

Status badges communicate workflow state at a glance.

Core badge set:

- `Pending`
- `Completed`
- `Failed`
- `Warning`
- `Info`

Badge rules:

- Use a pill shape.
- Keep the label short.
- Pair each badge with a consistent semantic color.
- Use the badge color as a state signal, not as decoration.

## Alerts

Alerts should communicate a problem or outcome without overwhelming the page.

Alert variants:

- Success
- Info
- Warning
- Error

Alert rules:

- Use a soft tinted background and a slightly stronger border.
- Keep the message concise.
- Place the most important sentence first.
- Reserve error alerts for actionable failures, not every negative state.

## Empty States

Empty states should help the user understand what is missing and what to do next.

Empty state rules:

- Explain the current state in one sentence.
- Offer one primary action when appropriate.
- Do not use decorative copy that distracts from the workflow.
- Keep tone direct and operational.

Examples:

- No business partners yet.
- No integration logs available.
- No failed integrations to retry.

## Loading States

Loading states should confirm that the system is working without causing layout jump.

Loading rules:

- Keep page structure stable while data loads.
- Prefer skeleton blocks for tables and dashboard cards.
- Use subtle activity indicators.
- Avoid long spinner-only screens.

Recommended behavior:

- Use a compact loading indicator for refresh actions.
- Use skeleton rows for tables during initial fetch.
- Disable submit buttons while requests are in flight.

## Responsive Rules

Fjordvia should work cleanly from mobile to desktop.

Responsive rules:

- Collapse multi-column grids into a single column below roughly `900px`.
- Keep forms stacked on smaller screens.
- Preserve table readability with horizontal scrolling instead of forcing tiny columns.
- Keep buttons and inputs full-width on narrow viewports when it improves usability.
- Maintain the same visual hierarchy across breakpoints.

Breakpoint guidance:

- Small: below `560px`
- Medium: `561px` to `900px`
- Large: above `900px`

## Motion

Motion should be subtle and purposeful.

Motion rules:

- Use short transitions for hover, focus, and expand states.
- Avoid excessive animation in the dashboard itself.
- If motion is used, it should support clarity, not entertainment.

## Implementation Notes

- Keep tokens centralized so frontend and future design work stay aligned.
- Reuse the same color and spacing variables across Angular and any future marketing or docs surfaces.
- Prefer composable components that can be reused for the dashboard, future admin views, and operational detail screens.
- Keep the UI premium, utilitarian, and calm.
