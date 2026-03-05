using DelegatesAndEvents.Delegates;
using DelegatesAndEvents.Events;

Console.WriteLine("=== Week 1 — Delegates and Events ===");

// --- Delegates ---
Console.WriteLine("\n--- Delegates ---");

// DELEGATES
var delegateBasics = new DelegateBasics();
delegateBasics.Run();

var buildInDelegates = new BuildInDelegates();
buildInDelegates.Run();

// EVENTS
var eventBasics = new EventBasics();
eventBasics.Run();

// Custom events with custom event args
var customEvents = new CustomEventPublisher();
var customSubscriber = new CustomEventSubscriber();
customSubscriber.Subscribe(customEvents);
customEvents.RaiseCustomEvent("Hello, this is a custom event!");

// Standard .NET event pattern with EventHandler and EventArgs
var standardPublisher = new StandardEventPublisher();
var standardSubscriber = new StandardEventSubscriber();
standardSubscriber.Subscribe(standardPublisher);
standardPublisher.RaiseStandardEvent();