-- Sample data for testing dashboard functionality
-- Run this after creating the views to have data for dashboard testing

-- Insert sample classes if they don't exist
INSERT INTO classes (id, name, grade, section, created_at) VALUES 
(uuid_generate_v4(), 'Grade 1A', '1', 'A', NOW()),
(uuid_generate_v4(), 'Grade 1B', '1', 'B', NOW()),
(uuid_generate_v4(), 'Grade 2A', '2', 'A', NOW()),
(uuid_generate_v4(), 'Grade 2B', '2', 'B', NOW()),
(uuid_generate_v4(), 'Grade 3A', '3', 'A', NOW())
ON CONFLICT DO NOTHING;

-- Insert sample students if they don't exist
DO $$
DECLARE 
    class_ids UUID[];
    class_id UUID;
    i INTEGER;
BEGIN
    -- Get all class IDs
    SELECT ARRAY_AGG(id) INTO class_ids FROM classes LIMIT 5;
    
    -- Insert students for each class
    FOR i IN 1..ARRAY_LENGTH(class_ids, 1) LOOP
        class_id := class_ids[i];
        
        -- Insert 25 students per class
        FOR j IN 1..25 LOOP
            INSERT INTO students (
                id, edunest_id, first_name, last_name, status, 
                classid, created_at, admission_number, phone_number, email
            ) VALUES (
                uuid_generate_v4(),
                'ST' || LPAD((i-1)*25 + j::TEXT, 6, '0'),
                'Student' || ((i-1)*25 + j)::TEXT,
                'LastName' || ((i-1)*25 + j)::TEXT,
                CASE WHEN j <= 23 THEN 'Active' ELSE 'Inactive' END,
                class_id,
                NOW(),
                'ADM' || LPAD(((i-1)*25 + j)::TEXT, 6, '0'),
                '+1234567' || LPAD(((i-1)*25 + j)::TEXT, 3, '0'),
                'student' || ((i-1)*25 + j)::TEXT || '@school.edu'
            ) ON CONFLICT (edunest_id) DO NOTHING;
        END LOOP;
    END LOOP;
END $$;

-- Insert sample staff if they don't exist
INSERT INTO staff (
    id, staff_id, first_name, last_name, role, joining_date, 
    status, phone, official_email, created_at
) VALUES 
(uuid_generate_v4(), 'TCH001', 'John', 'Smith', 'Teacher', '2023-01-15', 'Active', '+1234567890', 'john.smith@school.edu', NOW()),
(uuid_generate_v4(), 'TCH002', 'Sarah', 'Johnson', 'Teacher', '2023-02-01', 'Active', '+1234567891', 'sarah.johnson@school.edu', NOW()),
(uuid_generate_v4(), 'TCH003', 'Michael', 'Brown', 'Teacher', '2023-01-20', 'Active', '+1234567892', 'michael.brown@school.edu', NOW()),
(uuid_generate_v4(), 'ADM001', 'Emily', 'Davis', 'Administrator', '2022-08-15', 'Active', '+1234567893', 'emily.davis@school.edu', NOW()),
(uuid_generate_v4(), 'TCH004', 'David', 'Wilson', 'Teacher', '2023-03-01', 'On Leave', '+1234567894', 'david.wilson@school.edu', NOW()),
(uuid_generate_v4(), 'TCH005', 'Lisa', 'Miller', 'Teacher', '2022-09-01', 'Active', '+1234567895', 'lisa.miller@school.edu', NOW()),
(uuid_generate_v4(), 'SUP001', 'Robert', 'Taylor', 'Support Staff', '2023-01-10', 'Active', '+1234567896', 'robert.taylor@school.edu', NOW()),
(uuid_generate_v4(), 'TCH006', 'Jennifer', 'Anderson', 'Teacher', '2022-08-20', 'Terminated', '+1234567897', 'jennifer.anderson@school.edu', NOW()),
(uuid_generate_v4(), 'LIB001', 'William', 'Thomas', 'Librarian', '2022-07-01', 'Active', '+1234567898', 'william.thomas@school.edu', NOW()),
(uuid_generate_v4(), 'TCH007', 'Amanda', 'Jackson', 'Teacher', '2023-02-15', 'Active', '+1234567899', 'amanda.jackson@school.edu', NOW())
ON CONFLICT (staff_id) DO NOTHING;

-- Insert sample attendance data for the last 30 days
DO $$
DECLARE 
    student_record RECORD;
    current_date_iter DATE;
    class_ids UUID[];
BEGIN
    -- Get class IDs
    SELECT ARRAY_AGG(id) INTO class_ids FROM classes;
    
    -- Generate attendance for last 30 days (excluding weekends)
    FOR i IN 0..29 LOOP
        current_date_iter := CURRENT_DATE - INTERVAL '1 day' * i;
        
        -- Skip weekends
        IF EXTRACT(DOW FROM current_date_iter) NOT IN (0, 6) THEN
            -- Insert attendance for each active student
            FOR student_record IN 
                SELECT edunest_id, classid FROM students WHERE status = 'Active'
            LOOP
                INSERT INTO attendance (
                    id, edunest_id, class_id, date, is_present, created_at
                ) VALUES (
                    uuid_generate_v4(),
                    student_record.edunest_id,
                    student_record.classid,
                    current_date_iter,
                    -- 85% attendance rate with some randomness
                    CASE WHEN RANDOM() < 0.85 THEN TRUE ELSE FALSE END,
                    current_date_iter + INTERVAL '8 hours'
                ) ON CONFLICT DO NOTHING;
            END LOOP;
        END IF;
    END LOOP;
END $$;

-- Insert sample fee admin data
DO $$
DECLARE 
    class_record RECORD;
BEGIN
    FOR class_record IN SELECT id FROM classes LOOP
        INSERT INTO fee_admin (
            id, class_id, academic_year, monthly_fee, annual_fee, 
            admission_fee, transport_fee, library_fee, sports_fee, 
            miscellaneous_fee, is_active, created_at, updated_at
        ) VALUES (
            uuid_generate_v4(),
            class_record.id,
            '2025-26',
            500.00,  -- monthly fee
            5000.00, -- annual fee  
            1000.00, -- admission fee
            200.00,  -- transport fee
            100.00,  -- library fee
            150.00,  -- sports fee
            250.00,  -- miscellaneous fee
            TRUE,
            NOW(),
            NOW()
        ) ON CONFLICT (class_id, academic_year) DO NOTHING;
    END LOOP;
END $$;

-- Verify the data
SELECT 
    'Students' as entity,
    COUNT(*) as total_count,
    COUNT(CASE WHEN status = 'Active' THEN 1 END) as active_count
FROM students
UNION ALL
SELECT 
    'Staff' as entity,
    COUNT(*) as total_count,
    COUNT(CASE WHEN status = 'Active' THEN 1 END) as active_count
FROM staff
UNION ALL
SELECT 
    'Attendance Records' as entity,
    COUNT(*) as total_count,
    COUNT(CASE WHEN is_present = TRUE THEN 1 END) as present_count
FROM attendance;

-- Show recent attendance summary
SELECT 
    date,
    COUNT(*) as total_marked,
    COUNT(CASE WHEN is_present = TRUE THEN 1 END) as present,
    COUNT(CASE WHEN is_present = FALSE THEN 1 END) as absent,
    ROUND(
        COUNT(CASE WHEN is_present = TRUE THEN 1 END) * 100.0 / COUNT(*), 
        2
    ) as attendance_percentage
FROM attendance
WHERE date >= CURRENT_DATE - INTERVAL '7 days'
GROUP BY date
ORDER BY date DESC;