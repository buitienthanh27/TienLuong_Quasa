# 🛣️ IMPLEMENTATION ROADMAP
## Kế Hoạch Triển Khai Hệ Thống Tính Lương

---

## 1. EXECUTIVE SUMMARY

**Mục Tiêu:** Xây dựng hệ thống quản lý lương chuyên nghiệp, thay thế Excel spreadsheet hiện tại

**Trạng Thái Hiện Tại:**
- ❌ Process: Manual, error-prone, nhiều công thức Excel phức tạp
- ⚠️ Issues: 100+ #REF! errors, 10+ broken external links, không có version control
- ✅ Documentation: Đã phân tích chi tiết 5 sheets chính, 2,193 formulas

**Đề Xuất Tổng Thể:**
- **Timeline:** 4-6 tháng (từ analysis → production)
- **Team Size:** 4-5 người (BA, Dev, QA, DBA, DevOps)
- **Budget Estimate:** TBD (tùy theo infrastructure choice)
- **Risk Level:** MEDIUM (complex payroll logic, tax compliance)

---

## 2. PHASE BREAKDOWN

### 2.1 PHASE 1: Discovery & Design (Weeks 1-4)

**Objective:** Hoàn tất requirements, design, database schema

**Tasks:**

```
├─ 1.1 Validate All Requirements
│  ├─ Confirm P7 parameter value & purpose
│  ├─ Verify Grade D/E coefficients (currently unknown)
│  ├─ Validate tax thresholds against Vietnam 2025 tax law
│  ├─ Get approval on allocation percentages
│  └─ Confirm processing schedule & SLA
│
├─ 1.2 Design Database Schema
│  ├─ Finalize ERD based on normalized form
│  ├─ Design indexes for performance
│  ├─ Plan partitioning strategy (by month/employee)
│  ├─ Design audit trail & change tracking
│  └─ Document backup/recovery strategy
│
├─ 1.3 Design System Architecture
│  ├─ Choose tech stack (Java/Python/C#/.NET)
│  ├─ API design (REST/GraphQL)
│  ├─ Choose database (MySQL/PostgreSQL/SQL Server)
│  ├─ Plan security (auth, encryption, audit)
│  └─ Design report generation engine
│
├─ 1.4 Create Detailed Specifications
│  ├─ Business requirements document (BRD)
│  ├─ Technical requirements document (PRD)
│  ├─ Data migration plan
│  ├─ Testing strategy & test plan
│  └─ Training & documentation plan
│
└─ 1.5 Get Stakeholder Approvals
   ├─ Finance team review
   ├─ HR team review
   ├─ IT security review
   └─ Management sign-off
```

**Deliverables:**
- ✅ Confirmed BRD & PRD documents
- ✅ Database schema SQL scripts
- ✅ System architecture diagram
- ✅ Missing requirements clarification (P7, grade D/E, etc.)
- ✅ Project plan & risk register

**Success Criteria:**
- All requirements documented & approved
- Schema designed & reviewed by DBA
- Risk mitigation strategy in place

---

### 2.2 PHASE 2: Core Development (Weeks 5-12)

**Objective:** Build core payroll calculation engine

**Tasks:**

```
├─ 2.1 Database Setup
│  ├─ Create database & schemas
│  ├─ Create all tables from PRD
│  ├─ Add indexes & constraints
│  ├─ Create system_parameters table with defaults
│  ├─ Load reference data (grades, cost centers)
│  └─ Setup backup & monitoring
│
├─ 2.2 Data Migration Layer
│  ├─ Create Excel data importers
│  │  ├─ Employee data (MSNV 2025)
│  │  ├─ Attendance data (CHẤM CÔNG)
│  │  ├─ Performance data (sản lượng)
│  │  └─ Salary scale (CÁCH TÍNH LƯƠNG)
│  ├─ Validate imported data
│  ├─ Reconcile with Excel source
│  └─ Create rollback procedures
│
├─ 2.3 Core Calculation Engine
│  ├─ Phase 1: Performance Adjustment
│  │  ├─ Load grade coefficients
│  │  ├─ Calculate adjusted_output = output × grade_coef
│  │  └─ Store results
│  │
│  ├─ Phase 2: Base Salary Lookup
│  │  ├─ Load salary scale
│  │  └─ Store reference values
│  │
│  ├─ Phase 3: Main Salary Calculation
│  │  ├─ Formula: (base + days) / P7 × coef
│  │  ├─ Unit tests (target: 99% pass)
│  │  └─ Regression tests vs Excel data
│  │
│  ├─ Phase 4: Progressive Tax
│  │  ├─ Implement 6-tier tax logic
│  │  ├─ Unit tests (edge cases)
│  │  └─ Excel formula validation
│  │
│  └─ Phase 5: Deductions
│     ├─ Calculate BHXH (8%)
│     ├─ Calculate BHYT (1.5%)
│     ├─ Net salary = gross - tax - deductions
│     └─ Validation checks
│
├─ 2.4 Cost Allocation Engine
│  ├─ Load allocation rates from DB
│  ├─ Implement rounding algorithm (sum = total)
│  ├─ Generate allocation records
│  └─ Validate completeness
│
├─ 2.5 API Layer
│  ├─ Design REST endpoints
│  ├─ Implement payroll calculation API
│  ├─ Implement allocation API
│  ├─ Add error handling & logging
│  └─ API documentation (Swagger)
│
└─ 2.6 Testing & Verification
   ├─ Unit tests (>95% coverage)
   ├─ Integration tests
   ├─ Regression tests vs Excel
   ├─ Performance tests (1000+ employees)
   └─ UAT data preparation
```

**Deliverables:**
- ✅ Database ready in staging
- ✅ All core calculation modules working
- ✅ API endpoints functional
- ✅ Test suite with >95% pass rate
- ✅ Migration scripts for all Excel data

**Success Criteria:**
- All unit tests pass (>95% code coverage)
- Calculations match Excel within ±1 VND
- API response time < 100ms per employee
- Can process 1000 employees in < 5 minutes

---

### 2.3 PHASE 3: Reporting & UI (Weeks 13-16)

**Objective:** Build reporting engine & user interface

**Tasks:**

```
├─ 3.1 Reporting Engine
│  ├─ Monthly payroll report
│  │  ├─ By employee (gross, tax, net)
│  │  ├─ Summary statistics (totals, averages)
│  │  ├─ Tax bracket analysis
│  │  └─ Export (PDF, Excel, CSV)
│  │
│  ├─ Cost allocation report
│  │  ├─ By department
│  │  ├─ Reconciliation (sum = total ✓)
│  │  └─ Trend analysis
│  │
│  ├─ Audit reports
│  │  ├─ Change log (who changed what when)
│  │  ├─ Manual adjustment tracking
│  │  └─ Exception report (#REF! errors, anomalies)
│  │
│  └─ Dashboard
│     ├─ KPIs (total payroll, average salary, tax %)
│     ├─ Charts (salary distribution, tax breakdown)
│     └─ Alerts (errors, missing data, threshold breaches)
│
├─ 3.2 User Interface
│  ├─ Design mockups/wireframes
│  ├─ Build web frontend (React/Angular/Vue)
│  │  ├─ Payroll entry page
│  │  ├─ Employee management
│  │  ├─ Reporting dashboard
│  │  ├─ Parameter configuration
│  │  └─ Audit trail viewer
│  │
│  ├─ Build admin backoffice
│  │  ├─ Data import tools
│  │  ├─ System configuration
│  │  ├─ User management
│  │  └─ Access control
│  │
│  └─ Responsive design (desktop, tablet)
│
├─ 3.3 Report Generation
│  ├─ Choose report engine (Jasper, SSRS, etc.)
│  ├─ Design templates
│  ├─ Implement scheduled report generation
│  └─ Excel/PDF export with formatting
│
└─ 3.4 Testing
   ├─ UI/UX testing
   ├─ Report accuracy testing
   ├─ Performance testing (report generation)
   └─ Accessibility testing
```

**Deliverables:**
- ✅ All reports functional & tested
- ✅ Web UI deployed in staging
- ✅ Report templates ready
- ✅ User acceptance testing (UAT) packages

**Success Criteria:**
- All reports generate correctly
- UI accessible to target users
- Report generation < 1 minute for 1000 employees
- 100% of required reports implemented

---

### 2.4 PHASE 4: Security, Testing & Documentation (Weeks 17-20)

**Objective:** Complete security, exhaustive testing, documentation

**Tasks:**

```
├─ 4.1 Security Implementation
│  ├─ Authentication (AD/LDAP/OAuth)
│  ├─ Authorization (role-based access control)
│  ├─ Data encryption (at rest & in transit)
│  ├─ API security (API keys, rate limiting)
│  ├─ SQL injection prevention
│  ├─ XSS/CSRF protection
│  ├─ Audit logging (all data changes)
│  ├─ Compliance checks (tax, labor laws)
│  └─ Security testing (penetration test)
│
├─ 4.2 Comprehensive Testing
│  ├─ Functional testing
│  │  ├─ All use cases covered
│  │  ├─ Edge cases & boundary values
│  │  └─ Error scenarios
│  │
│  ├─ Performance testing
│  │  ├─ Load testing (100-1000 concurrent users)
│  │  ├─ Stress testing (peak loads)
│  │  ├─ Volume testing (years of data)
│  │  └─ Capacity planning
│  │
│  ├─ UAT (User Acceptance Testing)
│  │  ├─ Prepare UAT scenarios
│  │  ├─ Run with finance/HR team
│  │  ├─ Gather feedback
│  │  └─ Fix issues
│  │
│  └─ Regression testing
│     ├─ Automated test suite
│     ├─ Excel data validation
│     └─ Historical data reconciliation
│
├─ 4.3 Documentation
│  ├─ User manual
│  │  ├─ How to enter payroll data
│  │  ├─ How to run reports
│  │  ├─ Troubleshooting guide
│  │  └─ FAQ
│  │
│  ├─ Administrator guide
│  │  ├─ How to configure system
│  │  ├─ How to manage users
│  │  ├─ Backup/recovery procedures
│  │  └─ Maintenance schedule
│  │
│  ├─ Technical documentation
│  │  ├─ Architecture overview
│  │  ├─ Database schema
│  │  ├─ API documentation
│  │  ├─ Code documentation
│  │  └─ Deployment guide
│  │
│  └─ Training materials
│     ├─ User training videos
│     ├─ Admin training
│     ├─ QA testing guide
│     └─ Troubleshooting playbook
│
└─ 4.4 Data Cleanup (Fix Excel Issues)
   ├─ Create script to fix #REF! errors
   ├─ Rebuild external link references
   ├─ Validate all coefficients
   ├─ Migration test (Oct/Nov/Dec 2024 data)
   └─ Sign-off from business
```

**Deliverables:**
- ✅ Security assessment completed & issues resolved
- ✅ All tests passed (>99% coverage)
- ✅ UAT sign-off from business
- ✅ Complete documentation package
- ✅ Training materials

**Success Criteria:**
- All security tests pass
- UAT pass rate 100%
- Zero critical/high-priority bugs
- Team trained & ready for production

---

### 2.5 PHASE 5: Migration & Cutover (Week 21)

**Objective:** Move from Excel to production system

**Tasks:**

```
├─ 5.1 Pre-Cutover Activities
│  ├─ Final database validation
│  │  ├─ Verify all historical data loaded
│  │  ├─ Reconcile totals vs Excel
│  │  └─ Check for data integrity issues
│  │
│  ├─ Production environment setup
│  │  ├─ Production database
│  │  ├─ Production servers
│  │  ├─ Production backups
│  │  ├─ Production monitoring
│  │  └─ Disaster recovery plan
│  │
│  ├─ Parallel run (1 pay cycle)
│  │  ├─ Run system alongside Excel
│  │  ├─ Compare results
│  │  ├─ Investigate variances
│  │  └─ Fix any issues
│  │
│  └─ Cutover plan
│     ├─ Detailed runbook
│     ├─ Rollback procedures
│     ├─ Communication plan
│     ├─ On-call support schedule
│     └─ Success criteria
│
├─ 5.2 Production Deployment
│  ├─ Deploy application to production
│  ├─ Deploy database to production
│  ├─ Configure monitoring & alerts
│  ├─ Enable audit logging
│  └─ Verify all systems operational
│
├─ 5.3 Cutover Execution
│  ├─ Final historical data load
│  ├─ Switch from Excel to system
│  ├─ Run first production payroll
│  ├─ Verify results & reports
│  ├─ Approve for payment
│  └─ Payment distribution
│
├─ 5.4 Post-Go-Live Support
│  ├─ Monitor system performance
│  ├─ Address issues immediately
│  ├─ Gather user feedback
│  ├─ Optimize based on actual usage
│  └─ Transition to support team
│
└─ 5.5 Post-Mortem
   ├─ Lessons learned meeting
   ├─ Document issues & resolutions
   ├─ Performance metrics review
   ├─ Plan for Phase 2 enhancements
   └─ Archive documentation
```

**Deliverables:**
- ✅ Production system operational
- ✅ Historical data migrated
- ✅ First production payroll run successfully
- ✅ Support team trained

**Success Criteria:**
- System processes all employees correctly
- All reports accurate
- Parallel run variance ≤ 0.01%
- Zero production outages in first week

---

## 3. PHASE 2+ ENHANCEMENTS (Future)

```
├─ Employee self-service portal
│  ├─ View payslips (downloadable PDF)
│  ├─ View tax information
│  ├─ Update personal information
│  └─ Download tax certificate
│
├─ Advanced reporting
│  ├─ Custom report builder
│  ├─ Data export for tax filing
│  ├─ Government compliance reports
│  └─ Predictive analytics
│
├─ Integration with other systems
│  ├─ Accounting (QuickBooks, SAP)
│  ├─ ERP system
│  ├─ HR management system
│  ├─ Time tracking system
│  └─ Bank transfer integration (ACH/Swift)
│
├─ Advanced features
│  ├─ Bonus & incentive management
│  ├─ Allowance/deduction management
│  ├─ Budget vs. actual analysis
│  ├─ Predictive payroll forecasting
│  └─ Payroll process automation (RPA)
│
└─ Mobile app
   ├─ Employee self-service (mobile)
   ├─ Manager approvals
   ├─ Push notifications
   └─ Offline mode
```

---

## 4. RISK MANAGEMENT

### 4.1 High-Risk Items

| Risk | Probability | Impact | Mitigation |
|---|---|---|---|
| **Tax law changes** | Medium | High | Monitor tax law updates, maintain flexible config |
| **Data migration issues** | Medium | High | Extensive testing, parallel run, rollback plan |
| **Performance issues** | Low | Medium | Load testing, index optimization, caching |
| **User adoption** | Medium | Medium | Training, documentation, support team |
| **Data quality** | High | High | Data validation, cleansing, reconciliation |
| **Coefficient values missing** | Low | High | Confirm Grade D/E values immediately |
| **Integration challenges** | Medium | Medium | Early testing with HR/Finance systems |
| **Scope creep** | Medium | Medium | Strict change control, prioritize enhancements |

### 4.2 Mitigation Strategies

```
1. Tax Compliance Risk
   - Monthly review of tax law changes
   - Maintain parameter flexibility
   - Document all tax logic with sources
   - Get tax advisor review before major change

2. Data Migration Risk
   - Extensive data validation scripts
   - Parallel run for 1 full cycle
   - Rollback procedures documented
   - UAT with actual users

3. Performance Risk
   - Load test with 5000 employees
   - Database indexing strategy
   - Query optimization review
   - Monitor production metrics

4. User Adoption Risk
   - Comprehensive training (5+ sessions)
   - Detailed user documentation
   - 24/7 support first month
   - Collect feedback & iterate
```

---

## 5. RESOURCE PLANNING

### 5.1 Team Composition

```
Frontend Developer (1)
  ├─ React/Vue UI development
  ├─ Dashboard/reports UI
  └─ Responsive design

Backend Developer (2)
  ├─ Core calc engine
  ├─ API development
  ├─ Database layer
  └─ Integration code

Database Admin (1)
  ├─ Schema design
  ├─ Performance tuning
  ├─ Backup/recovery
  └─ Monitoring setup

Business Analyst (1)
  ├─ Requirements gathering
  ├─ Testing coordination
  ├─ Process documentation
  └─ Change management

QA Engineer (1)
  ├─ Test planning
  ├─ Test execution
  ├─ Bug tracking
  └─ UAT coordination

DevOps Engineer (0.5)
  ├─ Deployment automation
  ├─ Infrastructure
  ├─ Monitoring
  └─ Security hardening

Project Manager (1)
  ├─ Timeline management
  ├─ Risk management
  ├─ Stakeholder coordination
  └─ Budget tracking
```

### 5.2 Budget Estimate

```
Development:        4-6 months, 5 people = ~150K hours
Infrastructure:     Servers, DB licenses = 20-50K
Testing/QA:         Included in dev hours
Training:           Materials + sessions = 5-10K
Contingency (20%):  Budget for surprises

Total Estimate:     100-150K USD (exact depends on tech choice)
```

---

## 6. SUCCESS METRICS

```
Business Metrics:
  ✓ Error rate: < 0.1% (vs Excel ~2%)
  ✓ Processing time: < 2 hours for all employees
  ✓ User satisfaction: > 4.0/5.0
  ✓ Data accuracy: Match Excel within ±0.01%
  ✓ System uptime: > 99.5%

Technical Metrics:
  ✓ Test coverage: > 95%
  ✓ API response time: < 100ms
  ✓ Batch processing: < 5 min for 1000 employees
  ✓ Deployment time: < 30 min
  ✓ Zero critical bugs in production
  
Adoption Metrics:
  ✓ User training completion: 100%
  ✓ Support ticket resolution: < 24 hours
  ✓ User adoption rate: > 95% in month 1
  ✓ Data entry accuracy: > 99%
```

---

## 7. NEXT STEPS (Immediate)

**ACTION ITEMS (This Week):**

- [ ] 1. Confirm P7 parameter value & purpose with finance team
- [ ] 2. Get Grade D/E coefficient values from Excel or business
- [ ] 3. Validate tax thresholds against Vietnam 2025 tax code
- [ ] 4. Schedule kickoff meeting with stakeholders
- [ ] 5. Get approval for Phase 1 (Design & Discovery)
- [ ] 6. Allocate team members
- [ ] 7. Set up project management tool (Jira, Monday.com, etc.)
- [ ] 8. Schedule weekly sync meetings

**SPECIFIC QUESTIONS TO ANSWER:**

1. **P7 = 27.0**: What is the purpose? Is it adjustment factor? Can it change?
2. **Grade D & E**: What are the coefficients? (Currently unknown)
3. **Tax Law**: Are thresholds (5M, 10M, 20M, 25M, 65M) accurate for 2025?
4. **Allocation %**: Can the 30%, 25%, 20%, 10%, 10%, 5% be changed by user?
5. **Processing Schedule**: When is payroll processed? (Monthly, bi-weekly?)
6. **Payment Method**: How is payment distributed? (Bank transfer, cash, etc.)
7. **Regulatory Requirements**: Any specific reporting to government needed?
8. **Integration Needs**: Does this need to integrate with existing HR/ERP system?

---

**ROADMAP v1.0**
**Last Updated: 09/04/2025**
**Status: Ready for Stakeholder Review**

