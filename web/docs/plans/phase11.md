# Phase 11: Deployment & Operations

**Status**: Not started  
**Estimated Duration**: 1 week  
**Priority**: HIGH (enables production access)  
**Dependencies**: Phases 1-10 (all must be tested and stable)

## Goal
Set up production environment, deployment pipeline, monitoring, and operational procedures for stable and reliable system operation.

## Scope
- Production environment setup
- Deployment automation
- Monitoring and logging
- Backup and recovery procedures
- Performance optimization
- Security hardening
- Documentation and runbooks

## Detailed Tasks

### Infrastructure Setup

#### Task 1: Production Environment Configuration
**Location**: Configuration files and hosting provider

Steps:
1. Choose hosting provider:
   - Azure App Service (recommended for .NET)
   - AWS (EC2, RDS)
   - Google Cloud
   - or Self-hosted VPS

2. Set up production resources:
   - Web app hosting (frontend)
   - API hosting (backend)
   - SQL Server database
   - SSL certificate
   - Domain name
   - CDN (optional)

3. Create environment-specific appsettings:
   ```
   appsettings.Production.json (backend)
   .env.production (frontend)
   ```

4. Configure environment variables:
   - Connection strings (production database)
   - API URLs
   - Logging levels
   - Security keys/tokens
   - Email service credentials (if using)
   - Third-party API keys

**Technical Notes**:
- Use managed databases (Azure SQL, RDS)
- Store secrets in vault (Azure Key Vault, AWS Secrets Manager)
- Never commit secrets to repository
- Use different credentials for each environment

#### Task 2: Database Setup for Production
**Location**: Production SQL Server

Steps:
1. Create production database:
   - Database name: Medicare (or MedicareProduction)
   - Collation: SQL_Latin1_General_CP1_CI_AS
   - Recovery model: Full (for backup)
   - Growth settings: Auto-grow

2. Run migrations:
   ```
   dotnet ef database update --environment Production
   ```

3. Seed initial data:
   - Create default admin user
   - Create initial site content
   - Run seed scripts

4. Configure backups:
   - Automated daily backups
   - Backup retention: 30 days
   - Test restore procedures

5. Set up indexes:
   - Index on OrderDate, Status
   - Index on MenuDate, MealType
   - Index on User.Email
   - Regular index maintenance

**Technical Notes**:
- Use full recovery model for production
- Backup frequency: Daily at 2 AM
- Test backup restoration regularly
- Monitor database size and growth

#### Task 3: Configure HTTPS and Security
**Location**: Production server configuration

Steps:
1. Obtain SSL certificate:
   - Use Let's Encrypt (free)
   - Or purchase from certificate authority
   - Certificate valid for 1 year
   - Auto-renewal before expiry

2. Configure HTTPS:
   - Redirect HTTP to HTTPS
   - Set HSTS header (Strict-Transport-Security)
   - Configure TLS 1.2+
   - Disable weak ciphers

3. Set security headers:
   ```
   Content-Security-Policy
   X-Content-Type-Options: nosniff
   X-Frame-Options: DENY
   X-XSS-Protection: 1; mode=block
   Referrer-Policy: strict-origin-when-cross-origin
   ```

4. Configure CORS:
   - Only allow your frontend domain
   - Not wildcard (*)

**Technical Notes**:
- Renewal: 60 days before expiry
- Monitor certificate expiry
- Use strong cipher suites
- Keep TLS version current

#### Task 4: Configure Logging and Monitoring
**Location**: Application and monitoring services

Steps:
1. Set up application logging:
   - Structured logging (Serilog)
   - Log levels: Info (prod), Debug (dev)
   - Log to file and cloud service
   - Include request IDs for tracing

2. Configure log storage:
   - File system: `/var/log/medicare/` (Linux)
   - Or cloud: Application Insights, Datadog, etc.
   - Retention: 30 days minimum

3. Set up monitoring:
   - Monitor uptime (99%+ target)
   - Monitor response times (< 1 second)
   - Monitor error rates (< 0.1%)
   - Monitor CPU/Memory usage
   - Monitor database connections

4. Create dashboards:
   - Real-time metrics
   - Error tracking
   - User activity
   - Performance trends

5. Set up alerting:
   - High error rate (> 1%)
   - Service unavailable (downtime)
   - Database connection failures
   - Disk space low (< 10%)
   - SSL certificate expiry (< 14 days)

**Technical Notes**:
- Use Azure Application Insights (free tier available)
- Or use ELK stack (Elasticsearch, Logstash, Kibana)
- Log all API requests with response codes
- Log all errors with stack traces
- Correlate logs with request IDs

#### Task 5: Set Up Backup and Disaster Recovery
**Location**: Backup service and procedures

Steps:
1. Database backups:
   - Daily full backup (2 AM UTC)
   - Hourly incremental backups
   - Geo-redundant storage
   - Retention: 30 days
   - Test restore monthly

2. Application code backups:
   - Git repository (GitHub)
   - Release tags for each production version
   - Keep release history for 2 years

3. Recovery procedures:
   - Document RTO (Recovery Time Objective): < 1 hour
   - Document RPO (Recovery Point Objective): < 1 hour
   - Create runbook for disaster recovery
   - Test recovery procedure quarterly

4. Backup storage:
   - Separate geographic region
   - Encrypted at rest
   - Immutable (cannot delete/modify)
   - Automated tests to verify integrity

**Technical Notes**:
- RTO: < 1 hour (restore service)
- RPO: < 1 hour (data loss tolerance)
- Test backup restoration regularly
- Keep offline backup copy (optional)

### Deployment

#### Task 6: Create Deployment Scripts
**Location**: `scripts/deploy/`

Steps:
1. Create deployment script:
   ```bash
   #!/bin/bash
   # deploy.sh
   
   # Build backend
   cd backend
   dotnet build --configuration Release
   
   # Build frontend
   cd ../web
   npm run build
   
   # Deploy to hosting
   # (specifics depend on hosting provider)
   ```

2. Database migration script:
   ```bash
   #!/bin/bash
   # migrate-db.sh
   cd backend
   dotnet ef database update --environment Production
   ```

3. Rollback script:
   ```bash
   #!/bin/bash
   # rollback.sh
   # Revert to previous version
   # Steps depend on hosting provider
   ```

4. Test deployment in staging first

**Technical Notes**:
- Automate all deployment steps
- Use CI/CD pipeline (GitHub Actions, Azure DevOps)
- Always deploy to staging first
- Have rollback plan ready

#### Task 7: Set Up CI/CD Pipeline
**Location**: `.github/workflows/` or Azure DevOps

Steps:
1. Create deployment workflow:
   ```yaml
   name: Deploy to Production
   on:
     push:
       branches: [main]
   jobs:
     test:
       # Run all tests
     build:
       # Build application
     deploy:
       # Deploy to production (only if tests pass)
   ```

2. Steps in pipeline:
   - Run all tests
   - Run security scan
   - Build application
   - Deploy to staging
   - Run smoke tests
   - Deploy to production
   - Notify team

3. Approval gate:
   - Require manual approval for production deployment
   - Deployment only on business hours
   - Single deployer (to prevent race conditions)

**Technical Notes**:
- Use multi-stage deployment
- Test in staging before production
- Automate health checks post-deployment
- Notify team of deployments

#### Task 8: Document Deployment Procedures
**Location**: `docs/DEPLOYMENT.md`

Steps:
1. Create deployment guide:
   - Prerequisites
   - Step-by-step deployment instructions
   - Rollback procedures
   - Verification steps

2. Document configuration:
   - Required environment variables
   - Database connection strings
   - API endpoints
   - Third-party services

3. Document troubleshooting:
   - Common deployment issues
   - How to check logs
   - How to restore from backup
   - Emergency contacts

**Technical Notes**:
- Keep guide up-to-date
- Include screenshots if helpful
- Document all manual steps
- Include estimated deployment time (usually 10-20 min)

### Operations and Maintenance

#### Task 9: Create Operational Runbooks
**Location**: `docs/OPERATIONS.md`

Steps:
1. Create runbook for common tasks:
   - How to restart application
   - How to check system status
   - How to troubleshoot errors
   - How to scale resources
   - How to update certificates
   - How to restore from backup

2. Create on-call procedures:
   - Escalation path
   - Emergency contacts
   - Response time SLAs
   - Communication channels

3. Create change management:
   - Change request process
   - Approval workflow
   - Deployment window policies
   - Rollback criteria

**Technical Notes**:
- Keep runbooks simple and clear
- Include command examples
- Document all access procedures
- Regular training for ops team

#### Task 10: Set Up Monitoring Alerts
**Location**: Monitoring service configuration

Steps:
1. Configure alert channels:
   - Email notifications
   - Slack integration (optional)
   - SMS for critical alerts (optional)
   - PagerDuty integration (optional)

2. Create alert rules:
   - Error rate > 1%: Warning
   - Service unavailable: Critical
   - Response time > 5s: Warning
   - Database connection pool exhausted: Critical
   - Disk space < 10%: Warning
   - SSL certificate expires in 14 days: Warning

3. Create escalation policy:
   - First alert: Email all team members
   - 10 minutes: Page on-call engineer
   - 30 minutes: Escalate to manager

**Technical Notes**:
- Adjust thresholds based on actual metrics
- Avoid alert fatigue (false positives)
- Test alerts regularly
- Document alert meaning and actions

#### Task 11: Performance Optimization for Production
**Location**: Application and infrastructure configuration

Steps:
1. Enable caching:
   - Cache static assets (CDN)
   - Cache API responses (Redis optional)
   - Cache at database level

2. Optimize database:
   - Enable query result caching
   - Use connection pooling
   - Monitor slow queries
   - Regular index maintenance

3. Optimize frontend:
   - Minify and bundle assets
   - Enable gzip compression
   - Lazy load images
   - Cache in browser (1 year for versioned assets)

4. Monitor and adjust:
   - Monitor response times
   - Adjust cache policies
   - Optimize slow queries
   - Monitor resource usage

**Technical Notes**:
- Use profiling tools to identify bottlenecks
- A/B test optimizations
- Monitor performance metrics continuously

#### Task 12: Security Hardening
**Location**: Application and infrastructure

Steps:
1. Application security:
   - Enable HTTPS only
   - Set security headers
   - Enable CORS properly
   - Disable debug mode
   - Enable rate limiting (optional)

2. Database security:
   - Use strong passwords
   - Restrict database access
   - Enable encryption at rest
   - Enable encryption in transit

3. Infrastructure security:
   - Enable firewall rules
   - Restrict SSH/RDP access
   - Use VPN for admin access
   - Regular security updates

4. Monitoring security:
   - Monitor failed login attempts
   - Monitor unusual API activity
   - Scan for vulnerabilities regularly
   - Review access logs monthly

**Technical Notes**:
- Regular security audits
- Keep dependencies updated
- Use security scanning tools
- Employee security training

### Documentation

#### Task 13: Create Operations Documentation
**Location**: `docs/` folder

Steps:
1. Create comprehensive documentation:
   - System architecture diagram
   - Deployment architecture
   - Data flow diagrams
   - API documentation (auto-generated Swagger)
   - Database schema documentation
   - Component documentation

2. Create troubleshooting guide:
   - Common errors and solutions
   - Log interpretation
   - Performance issues
   - Data recovery procedures

3. Create user guides:
   - Admin user guide
   - Customer user guide
   - FAQ

**Technical Notes**:
- Keep documentation updated
- Use diagrams for clarity
- Include code examples
- Link related documents

## Acceptance Criteria
- [x] Application runs on production server
- [x] SSL certificate is installed and valid
- [x] Database backup runs automatically
- [x] All logs are captured and accessible
- [x] Performance monitoring is active
- [x] Alerts notify ops team of issues
- [x] Health checks endpoints respond
- [x] Deployment can be automated
- [x] Rollback procedures are documented
- [x] Recovery time objective (RTO) < 1 hour
- [x] Recovery point objective (RPO) < 1 hour
- [x] All documentation is current

## Testing Procedures

### Pre-Deployment Checks

1. **Build Verification**
   - Verify build completes without errors
   - Verify tests pass (100%)
   - Verify no security warnings

2. **Staging Deployment**
   - Deploy to staging environment
   - Run smoke tests
   - Verify all features work
   - Check performance metrics
   - Verify monitoring works

3. **Production Deployment**
   - Deploy during low-traffic period
   - Monitor error rates closely
   - Monitor performance metrics
   - Verify database backups
   - Verify logs are being recorded

### Post-Deployment Validation

1. **Health Checks**
   ```
   GET /health → 200 OK
   GET /api/menus → 200 OK (accessible)
   POST /api/orders → 400/201 (requires valid data)
   GET /admin → 401 (requires auth)
   ```

2. **Critical Flows**
   - Customer can submit order
   - Admin can log in
   - Admin can view orders
   - Admin can update menu

3. **Monitoring**
   - Check application metrics
   - Check error logs
   - Check database performance
   - Check disk usage
   - Check certificate validity

## Files to Create/Modify

### New Files
```
scripts/deploy/deploy.sh
scripts/deploy/migrate-db.sh
scripts/deploy/rollback.sh
.github/workflows/deploy.yml (CI/CD)
docs/DEPLOYMENT.md
docs/OPERATIONS.md
docs/OPERATIONS_RUNBOOK.md
docs/ARCHITECTURE_PRODUCTION.md
docs/MONITORING_ALERTS.md
.dockerignore (if using Docker)
Dockerfile (if using Docker)
docker-compose.yml (if using Docker)
```

### Modified Files
```
backend/appsettings.Production.json
web/.env.production
README.md (add deployment instructions)
web/.github/workflows/build-deploy.yml
```

## Dependencies
- Azure App Service (or alternative hosting)
- Azure SQL Database (or alternative)
- Azure Key Vault (for secrets)
- Application Insights (for monitoring)

## Related Documentation
- All phases must be complete and tested
- See [DEPLOYMENT.md](../DEPLOYMENT.md) for detailed steps
- See [OPERATIONS.md](../OPERATIONS.md) for procedures

## Notes
- Production readiness is critical
- Automate everything possible
- Monitor continuously
- Have backup plans for all critical components
- Regular disaster recovery drills recommended
- Plan for scaling if traffic grows significantly
