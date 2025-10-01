-- Admin Dashboard Views and Functions
-- These views provide consolidated data for the admin dashboard

-- 1. Student Statistics View
CREATE OR REPLACE VIEW v_student_statistics AS
SELECT 
    COUNT(*) as total_students,
    COUNT(CASE WHEN status = 'Active' THEN 1 END) as active_students,
    COUNT(CASE WHEN status != 'Active' THEN 1 END) as inactive_students,
    ROUND(
        COUNT(CASE WHEN status = 'Active' THEN 1 END) * 100.0 / NULLIF(COUNT(*), 0), 
        2
    ) as active_percentage
FROM students;

-- 2. Staff Statistics View  
CREATE OR REPLACE VIEW v_staff_statistics AS
SELECT 
    COUNT(*) as total_staff,
    COUNT(CASE WHEN status = 'Active' THEN 1 END) as active_staff,
    COUNT(CASE WHEN status = 'On Leave' THEN 1 END) as staff_on_leave,
    COUNT(CASE WHEN status = 'Terminated' THEN 1 END) as terminated_staff,
    COUNT(CASE WHEN status = 'Retired' THEN 1 END) as retired_staff,
    ROUND(
        COUNT(CASE WHEN status = 'Active' THEN 1 END) * 100.0 / NULLIF(COUNT(*), 0), 
        2
    ) as active_staff_percentage
FROM staff;

-- 3. Daily Attendance Summary View
CREATE OR REPLACE VIEW v_daily_attendance_summary AS
SELECT 
    date,
    COUNT(*) as total_marked,
    COUNT(CASE WHEN is_present = true THEN 1 END) as present_count,
    COUNT(CASE WHEN is_present = false THEN 1 END) as absent_count,
    ROUND(
        COUNT(CASE WHEN is_present = true THEN 1 END) * 100.0 / NULLIF(COUNT(*), 0), 
        2
    ) as attendance_percentage
FROM attendance 
GROUP BY date
ORDER BY date DESC;

-- 4. Current Month Attendance Summary
CREATE OR REPLACE VIEW v_current_month_attendance AS
SELECT 
    EXTRACT(YEAR FROM date) as year,
    EXTRACT(MONTH FROM date) as month,
    COUNT(*) as total_records,
    COUNT(CASE WHEN is_present = true THEN 1 END) as total_present,
    COUNT(CASE WHEN is_present = false THEN 1 END) as total_absent,
    COUNT(DISTINCT edunest_id) as unique_students,
    COUNT(DISTINCT date) as school_days,
    ROUND(
        COUNT(CASE WHEN is_present = true THEN 1 END) * 100.0 / NULLIF(COUNT(*), 0), 
        2
    ) as overall_attendance_percentage
FROM attendance 
WHERE date >= DATE_TRUNC('month', CURRENT_DATE)
  AND date <= CURRENT_DATE
GROUP BY EXTRACT(YEAR FROM date), EXTRACT(MONTH FROM date);

-- 5. Class-wise Attendance Summary (Current Month)
CREATE OR REPLACE VIEW v_class_attendance_summary AS
SELECT 
    c.grade,
    c.section,
    c.id as class_id,
    COUNT(a.*) as total_attendance_records,
    COUNT(CASE WHEN a.is_present = true THEN 1 END) as present_count,
    COUNT(CASE WHEN a.is_present = false THEN 1 END) as absent_count,
    COUNT(DISTINCT a.edunest_id) as students_with_records,
    COUNT(DISTINCT a.date) as days_recorded,
    ROUND(
        COUNT(CASE WHEN a.is_present = true THEN 1 END) * 100.0 / NULLIF(COUNT(a.*), 0), 
        2
    ) as class_attendance_percentage
FROM classes c
LEFT JOIN attendance a ON c.id = a.class_id 
WHERE a.date >= DATE_TRUNC('month', CURRENT_DATE)
  AND a.date <= CURRENT_DATE
GROUP BY c.id, c.grade, c.section
ORDER BY c.grade, c.section;

-- 6. Admin Dashboard Summary View (Main dashboard view)
CREATE OR REPLACE VIEW v_admin_dashboard_summary AS
SELECT 
    -- Student Statistics
    (SELECT active_students FROM v_student_statistics) as total_active_students,
    (SELECT total_students FROM v_student_statistics) as total_students,
    (SELECT active_percentage FROM v_student_statistics) as student_active_percentage,
    
    -- Staff Statistics  
    (SELECT active_staff FROM v_staff_statistics) as total_active_staff,
    (SELECT total_staff FROM v_staff_statistics) as total_staff,
    (SELECT active_staff_percentage FROM v_staff_statistics) as staff_active_percentage,
    
    -- Current Month Attendance
    (SELECT overall_attendance_percentage FROM v_current_month_attendance) as current_month_attendance_percentage,
    (SELECT school_days FROM v_current_month_attendance) as school_days_this_month,
    (SELECT unique_students FROM v_current_month_attendance) as students_with_attendance,
    
    -- Today's Attendance (if available)
    COALESCE((
        SELECT attendance_percentage 
        FROM v_daily_attendance_summary 
        WHERE date = CURRENT_DATE
    ), 0) as today_attendance_percentage,
    
    COALESCE((
        SELECT total_marked 
        FROM v_daily_attendance_summary 
        WHERE date = CURRENT_DATE
    ), 0) as today_total_marked;

-- 7. Recent Attendance Trends (Last 7 days)
CREATE OR REPLACE VIEW v_recent_attendance_trends AS
SELECT 
    date,
    attendance_percentage,
    total_marked,
    present_count,
    absent_count
FROM v_daily_attendance_summary
WHERE date >= CURRENT_DATE - INTERVAL '7 days'
ORDER BY date DESC;

-- 8. Function to get dashboard data for a specific date range
CREATE OR REPLACE FUNCTION get_attendance_summary(
    start_date DATE DEFAULT CURRENT_DATE - INTERVAL '30 days',
    end_date DATE DEFAULT CURRENT_DATE
)
RETURNS TABLE (
    total_school_days BIGINT,
    average_attendance_percentage NUMERIC,
    total_attendance_records BIGINT,
    unique_students_count BIGINT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(DISTINCT a.date) as total_school_days,
        ROUND(AVG(
            COUNT(CASE WHEN a.is_present = true THEN 1 END) * 100.0 / NULLIF(COUNT(a.*), 0)
        ), 2) as average_attendance_percentage,
        COUNT(a.*) as total_attendance_records,
        COUNT(DISTINCT a.edunest_id) as unique_students_count
    FROM attendance a
    WHERE a.date BETWEEN start_date AND end_date
    GROUP BY ()
    HAVING COUNT(a.*) > 0;
END;
$$ LANGUAGE plpgsql;

-- 9. Fee Collection Statistics View
CREATE OR REPLACE VIEW v_fee_statistics AS
SELECT 
    COUNT(*) as total_fee_records,
    SUM(fee_collected) as total_fee_collected,
    SUM(fee_waived) as total_fee_waived,
    SUM(fee_collected + fee_waived) as total_fee_due,
    COUNT(DISTINCT student_edunest_id) as students_with_fee_records,
    ROUND(AVG(fee_collected), 2) as average_fee_collected,
    ROUND(
        SUM(fee_collected) * 100.0 / NULLIF(SUM(fee_collected + fee_waived), 0), 
        2
    ) as fee_collection_percentage
FROM fees_collection;

-- 10. Current Month Fee Collection Summary
CREATE OR REPLACE VIEW v_current_month_fee_collection AS
SELECT 
    EXTRACT(YEAR FROM date_of_collection) as year,
    EXTRACT(MONTH FROM date_of_collection) as month,
    COUNT(*) as total_transactions,
    SUM(fee_collected) as total_collected_this_month,
    SUM(fee_waived) as total_waived_this_month,
    COUNT(DISTINCT student_edunest_id) as unique_students_paid,
    ROUND(AVG(fee_collected), 2) as average_collection_per_transaction,
    MAX(fee_collected) as highest_collection,
    MIN(fee_collected) as lowest_collection
FROM fees_collection 
WHERE date_of_collection >= DATE_TRUNC('month', CURRENT_DATE)
  AND date_of_collection <= CURRENT_DATE
GROUP BY EXTRACT(YEAR FROM date_of_collection), EXTRACT(MONTH FROM date_of_collection);

-- 11. Class-wise Fee Collection Summary
CREATE OR REPLACE VIEW v_class_fee_summary AS
SELECT 
    c.grade,
    c.section,
    c.id as class_id,
    c.name as class_name,
    COUNT(fc.*) as total_fee_transactions,
    SUM(fc.fee_collected) as total_collected,
    SUM(fc.fee_waived) as total_waived,
    COUNT(DISTINCT fc.student_edunest_id) as students_paid,
    COUNT(DISTINCT s.id) as total_students_in_class,
    ROUND(AVG(fc.fee_collected), 2) as average_fee_per_student,
    ROUND(
        COUNT(DISTINCT fc.student_edunest_id) * 100.0 / NULLIF(COUNT(DISTINCT s.id), 0), 
        2
    ) as payment_completion_percentage
FROM classes c
LEFT JOIN students s ON c.id = s.classid
LEFT JOIN fees_collection fc ON s.edunest_id = fc.student_edunest_id
GROUP BY c.id, c.grade, c.section, c.name
ORDER BY c.grade, c.section;

-- 12. Fee Admin Configuration Summary
CREATE OR REPLACE VIEW v_fee_admin_summary AS
SELECT 
    c.grade,
    c.section,
    c.name as class_name,
    fa.academic_year,
    fa.total_annual_fee,
    fa.monthly_fee,
    fa.is_active,
    COALESCE(cls_summary.total_collected, 0) as actual_collected,
    COALESCE(cls_summary.students_paid, 0) as students_paid,
    COALESCE(cls_summary.total_students_in_class, 0) as total_students,
    ROUND(
        COALESCE(cls_summary.total_collected, 0) * 100.0 / 
        NULLIF(fa.total_annual_fee * COALESCE(cls_summary.total_students_in_class, 1), 0), 
        2
    ) as collection_vs_target_percentage
FROM fee_admin fa
JOIN classes c ON fa.class_id = c.id
LEFT JOIN v_class_fee_summary cls_summary ON c.id = cls_summary.class_id
WHERE fa.is_active = true
ORDER BY c.grade, c.section, fa.academic_year;

-- 13. Daily Fee Collection Summary
CREATE OR REPLACE VIEW v_daily_fee_collection AS
SELECT 
    date_of_collection as collection_date,
    COUNT(*) as transactions_count,
    SUM(fee_collected) as total_collected,
    SUM(fee_waived) as total_waived,
    COUNT(DISTINCT student_edunest_id) as unique_students,
    ROUND(AVG(fee_collected), 2) as average_per_transaction
FROM fees_collection 
GROUP BY date_of_collection
ORDER BY date_of_collection DESC;

-- 14. Recent Fee Collection Trends (Last 7 days)
CREATE OR REPLACE VIEW v_recent_fee_trends AS
SELECT 
    collection_date,
    total_collected,
    transactions_count,
    unique_students,
    average_per_transaction
FROM v_daily_fee_collection
WHERE collection_date >= CURRENT_DATE - INTERVAL '7 days'
ORDER BY collection_date DESC;

-- 15. Outstanding Fees Summary (Students who haven't paid vs Fee Admin targets)
CREATE OR REPLACE VIEW v_outstanding_fees_summary AS
SELECT 
    c.grade,
    c.section,
    c.name as class_name,
    COUNT(s.id) as total_students,
    COUNT(CASE WHEN fc.student_edunest_id IS NOT NULL THEN 1 END) as students_paid,
    COUNT(CASE WHEN fc.student_edunest_id IS NULL THEN 1 END) as students_not_paid,
    COALESCE(fa.total_annual_fee, 0) as expected_fee_per_student,
    COALESCE(fa.total_annual_fee * COUNT(s.id), 0) as total_expected_collection,
    COALESCE(SUM(fc.fee_collected), 0) as actual_collection,
    COALESCE(fa.total_annual_fee * COUNT(CASE WHEN fc.student_edunest_id IS NULL THEN 1 END), 0) as outstanding_amount
FROM classes c
LEFT JOIN students s ON c.id = s.classid AND s.status = 'Active'
LEFT JOIN fee_admin fa ON c.id = fa.class_id AND fa.is_active = true
LEFT JOIN fees_collection fc ON s.edunest_id = fc.student_edunest_id
GROUP BY c.id, c.grade, c.section, c.name, fa.total_annual_fee
ORDER BY c.grade, c.section;

-- 16. Enhanced Admin Dashboard Summary View (including fee statistics)
CREATE OR REPLACE VIEW v_admin_dashboard_summary AS
SELECT 
    -- Student Statistics
    (SELECT active_students FROM v_student_statistics) as total_active_students,
    (SELECT total_students FROM v_student_statistics) as total_students,
    (SELECT active_percentage FROM v_student_statistics) as student_active_percentage,
    
    -- Staff Statistics  
    (SELECT active_staff FROM v_staff_statistics) as total_active_staff,
    (SELECT total_staff FROM v_staff_statistics) as total_staff,
    (SELECT active_staff_percentage FROM v_staff_statistics) as staff_active_percentage,
    
    -- Current Month Attendance
    (SELECT overall_attendance_percentage FROM v_current_month_attendance) as current_month_attendance_percentage,
    (SELECT school_days FROM v_current_month_attendance) as school_days_this_month,
    (SELECT students_with_attendance FROM v_current_month_attendance) as students_with_attendance,
    
    -- Today's Attendance (if available)
    COALESCE((
        SELECT attendance_percentage 
        FROM v_daily_attendance_summary 
        WHERE date = CURRENT_DATE
    ), 0) as today_attendance_percentage,
    
    COALESCE((
        SELECT total_marked 
        FROM v_daily_attendance_summary 
        WHERE date = CURRENT_DATE
    ), 0) as today_total_marked,
    
    -- Fee Statistics
    (SELECT total_fee_collected FROM v_fee_statistics) as total_fee_collected,
    (SELECT fee_collection_percentage FROM v_fee_statistics) as overall_fee_collection_percentage,
    (SELECT students_with_fee_records FROM v_fee_statistics) as students_with_fee_records,
    
    -- Current Month Fee Collection
    COALESCE((SELECT total_collected_this_month FROM v_current_month_fee_collection), 0) as current_month_fee_collected,
    COALESCE((SELECT unique_students_paid FROM v_current_month_fee_collection), 0) as current_month_students_paid,
    COALESCE((SELECT total_transactions FROM v_current_month_fee_collection), 0) as current_month_fee_transactions,
    
    -- Outstanding Fees
    (SELECT SUM(outstanding_amount) FROM v_outstanding_fees_summary) as total_outstanding_fees,
    (SELECT SUM(students_not_paid) FROM v_outstanding_fees_summary) as students_with_outstanding_fees;

-- 17. Function to get fee collection summary for a specific date range
CREATE OR REPLACE FUNCTION get_fee_collection_summary(
    start_date DATE DEFAULT CURRENT_DATE - INTERVAL '30 days',
    end_date DATE DEFAULT CURRENT_DATE
)
RETURNS TABLE (
    total_collection_days BIGINT,
    total_amount_collected NUMERIC,
    total_transactions BIGINT,
    unique_students_paid BIGINT,
    average_daily_collection NUMERIC,
    average_per_transaction NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(DISTINCT fc.date_of_collection) as total_collection_days,
        COALESCE(SUM(fc.fee_collected), 0) as total_amount_collected,
        COUNT(fc.*) as total_transactions,
        COUNT(DISTINCT fc.student_edunest_id) as unique_students_paid,
        ROUND(
            COALESCE(SUM(fc.fee_collected), 0) / NULLIF(COUNT(DISTINCT fc.date_of_collection), 0), 
            2
        ) as average_daily_collection,
        ROUND(
            COALESCE(SUM(fc.fee_collected), 0) / NULLIF(COUNT(fc.*), 0), 
            2
        ) as average_per_transaction
    FROM fees_collection fc
    WHERE fc.date_of_collection BETWEEN start_date AND end_date;
END;
$$ LANGUAGE plpgsql;

-- Grant permissions to the application user
GRANT SELECT ON v_student_statistics TO edunest_app;
GRANT SELECT ON v_staff_statistics TO edunest_app;
GRANT SELECT ON v_daily_attendance_summary TO edunest_app;
GRANT SELECT ON v_current_month_attendance TO edunest_app;
GRANT SELECT ON v_class_attendance_summary TO edunest_app;
GRANT SELECT ON v_admin_dashboard_summary TO edunest_app;
GRANT SELECT ON v_recent_attendance_trends TO edunest_app;
GRANT SELECT ON v_fee_statistics TO edunest_app;
GRANT SELECT ON v_current_month_fee_collection TO edunest_app;
GRANT SELECT ON v_class_fee_summary TO edunest_app;
GRANT SELECT ON v_fee_admin_summary TO edunest_app;
GRANT SELECT ON v_daily_fee_collection TO edunest_app;
GRANT SELECT ON v_recent_fee_trends TO edunest_app;
GRANT SELECT ON v_outstanding_fees_summary TO edunest_app;
GRANT EXECUTE ON FUNCTION get_attendance_summary(DATE, DATE) TO edunest_app;
GRANT EXECUTE ON FUNCTION get_fee_collection_summary(DATE, DATE) TO edunest_app;