using DelegatesAndEvents.Delegates;
using DelegatesAndEvents.Events;

// DELEGATES
var delegateBasics = new DelegateBasics();
delegateBasics.Run();

var buildInDelegates = new BuildInDelegates();
buildInDelegates.Run();

// EVENTS
var eventBasics = new EventBasics();
eventBasics.Run();

var customEvents = new CustomEventPublisher();
var customSubscriber = new CustomEventSubscriber();
customSubscriber.Subscribe(customEvents);
customEvents.RaiseCustomEvent("Hello, this is a custom event!");

var standardPublisher = new StandardEventPublisher();
var standardSubscriber = new StandardEventSubscriber();
standardSubscriber.Subscribe(standardPublisher);
standardPublisher.RaiseStandardEvent("Hello, this is a standard event!");