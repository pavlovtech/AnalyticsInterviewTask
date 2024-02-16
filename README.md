# Tasks

1. Review code & explain what it does
2. Identify issues
3. Propose improvements
4. Agree an improvements with interviewer and implement those

# Explanation
## Entities
- `User` & `Product` are DTOs for simulating contracts and behavioral concepts between them
- `OrderProcessorBase` class is an abstraction over ordering process. It defines sequence of operation using so-known 
`Template Method Pattern` via methods: `ProcessPayment` & `GenerateReceiptAsync`. Using `ProcessAsync` we guarantee that
first `ProcessPayment` method will be executed. Then and only then will be executed `GenerateReceiptAsync`. Factually, 
it defines the flow structure and inheritors only overrides abstract methods covering specification.
- `CashOrderProcessor` & `CreditCardOrderProcessor` are inheritors of `OrderProcessorBase`. They define specific 
strategies of payment: `Cash` and `Credit`.

## Runtime Behavior
- `GetCartProducts` method provides products in cart but throws exception in the middle of iteration (simulating that 
there is no available item).
- During the runtime we are running two "tries" for making order via `Cash` strategy and `Credit`.