---
title: "Control Plan Constraints"
date: 2022-12-14T05:44:45+05:30
draft: false
---

### Control Plan Constraints :
Following are the expectations & constraints imposed by the Traceability web-application , and need to be adhered by the QDAS control plan schema, to maintain the integrity of the solution : 
1. Each control plan created should have mandatorily following fields : 
    - K1001 : Part Number 
    - K1086 : Operation Number 
    - K1102 : Line Name 
1. A unique value in K1001 should be used to describe a combination of unique part type for a unique model 
1. K1086 should contain digits (0-9) within them to describe the operation stage. These numbers are used to identify the sequence of each operation within the given line 
2. To use K0055 to store the unique serail number / identification number (UID) of any component. It should capture the whole UID and not any of the partial value 
3. For measurement values, use K0008 to store the operator responsible and K0010 to store the machine involved in manufacutring/ inspecting certain component 
4. Traceability Web-Application assumes following mannerism in data storage within Qdas database: 
    - All characteristic's measurements are written down in database at once/ simultaneously (i.e without any practical time gap) involved within a specific operation stage
    - All characteristic's measurements will be tightly coupled to have the same values for these operation parameters such as K0008, K0010 & timestamp 
