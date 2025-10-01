# Admin Dashboard Implementation Guide

This guide explains how to implement a comprehensive admin dashboard showing attendance percentages, student counts, and staff statistics.

## Database Setup

### 1. Create Dashboard Views and Functions
Run the SQL script to create all necessary views and functions:
```bash
psql -d your_database -f Database/dashboard_views.sql
```

### 2. Insert Sample Data (Optional - for testing)
To test the dashboard with sample data:
```bash
psql -d your_database -f Database/sample_data.sql
```

## API Endpoints

The dashboard provides the following REST endpoints:

### Main Dashboard Data
- **GET** `/api/dashboard` - Complete dashboard with all data
- **GET** `/api/dashboard/summary` - Key metrics summary

### Detailed Views  
- **GET** `/api/dashboard/attendance-trends?days=7` - Recent attendance trends
- **GET** `/api/dashboard/class-attendance` - Class-wise attendance (current month)
- **GET** `/api/dashboard/attendance-summary?startDate=2025-01-01&endDate=2025-01-31` - Custom date range summary

## Dashboard Metrics

### Student Statistics
- Total active students
- Total students (all statuses)  
- Active student percentage

### Staff Statistics
- Total active staff
- Total staff (all statuses)
- Active staff percentage  

### Attendance Analytics
- Current month attendance percentage
- Today's attendance percentage
- School days this month
- Students with attendance records
- Recent attendance trends (last 7 days)
- Class-wise attendance breakdown

## Response Examples

### Dashboard Summary Response
```json
{
  "totalActiveStudents": 115,
  "totalStudents": 125,
  "studentActivePercentage": 92.00,
  "totalActiveStaff": 8,
  "totalStaff": 10,
  "staffActivePercentage": 80.00,
  "currentMonthAttendancePercentage": 87.50,
  "schoolDaysThisMonth": 20,
  "studentsWithAttendance": 115,
  "todayAttendancePercentage": 89.00,
  "todayTotalMarked": 115
}
```

### Complete Dashboard Response
```json
{
  "summary": {
    // ... same as above
  },
  "recentTrends": [
    {
      "date": "2025-09-28",
      "attendancePercentage": 89.00,
      "totalMarked": 115,
      "presentCount": 102,
      "absentCount": 13
    }
    // ... more days
  ],
  "classAttendance": [
    {
      "grade": "1",
      "section": "A",
      "classId": "uuid-here",
      "totalAttendanceRecords": 460,
      "presentCount": 391,
      "absentCount": 69,
      "studentsWithRecords": 23,
      "daysRecorded": 20,
      "classAttendancePercentage": 85.00
    }
    // ... more classes
  ]
}
```

## Database Views Created

### Core Views
- `v_admin_dashboard_summary` - Main dashboard metrics
- `v_student_statistics` - Student count and status breakdown  
- `v_staff_statistics` - Staff count and status breakdown
- `v_daily_attendance_summary` - Daily attendance percentages
- `v_current_month_attendance` - Current month attendance analytics
- `v_class_attendance_summary` - Class-wise attendance breakdown
- `v_recent_attendance_trends` - Last 7 days attendance trends

### Helper Function
- `get_attendance_summary(start_date, end_date)` - Custom date range analytics

## Frontend Integration

### Sample Dashboard Component (React/Vue)
```javascript
// Fetch complete dashboard data
const fetchDashboardData = async () => {
  try {
    const response = await fetch('/api/dashboard', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    const data = await response.json();
    
    // Use data.summary, data.recentTrends, data.classAttendance
    console.log('Dashboard data:', data);
  } catch (error) {
    console.error('Error fetching dashboard:', error);
  }
};
```

### Key Metrics Cards
- **Students**: Show `totalActiveStudents` / `totalStudents` with percentage
- **Staff**: Show `totalActiveStaff` / `totalStaff` with percentage  
- **Today's Attendance**: Show `todayAttendancePercentage`% of `todayTotalMarked` students
- **Monthly Average**: Show `currentMonthAttendancePercentage`% over `schoolDaysThisMonth` days

### Charts/Graphs
- **Attendance Trends**: Line chart using `recentTrends` data
- **Class Performance**: Bar chart using `classAttendance` data
- **Status Breakdown**: Pie charts for student/staff active vs inactive

## Performance Considerations

### Database Optimization
- Views use indexed columns (`class_id`, `date`, `status`)
- Attendance queries limited to reasonable date ranges
- Cached results in memory cache for frequently accessed data

### Caching Strategy
```csharp
// Example: Cache dashboard summary for 5 minutes
[ResponseCache(Duration = 300)]
public async Task<IActionResult> GetDashboardSummary()
```

## Security & Access Control

### Authorization
Add role-based access control:
```csharp
[Authorize(Roles = "Admin,Principal")]
public class DashboardController : ControllerBase
```

### Data Filtering
- All queries respect tenant isolation
- Staff can only see their class data
- Admins see school-wide statistics

## Monitoring & Logging

The implementation includes comprehensive logging:
- Request/response logging for all endpoints
- Performance metrics for database queries  
- Error handling with detailed logs
- Dashboard usage analytics

## Troubleshooting

### Common Issues

1. **No data showing**: Check if sample data was inserted and views created
2. **Permission errors**: Ensure `edunest_app` user has SELECT permissions on views
3. **Tenant issues**: Verify JWT token contains correct `tenantId` claim
4. **Date range errors**: Check date parameters are valid and not too large

### Debug Queries
```sql
-- Test dashboard summary view
SELECT * FROM v_admin_dashboard_summary;

-- Check attendance data exists
SELECT COUNT(*) FROM attendance WHERE date >= CURRENT_DATE - INTERVAL '7 days';

-- Verify student/staff counts
SELECT status, COUNT(*) FROM students GROUP BY status;
SELECT status, COUNT(*) FROM staff GROUP BY status;
```

This implementation provides a comprehensive admin dashboard that scales with your data and provides valuable insights into school operations.