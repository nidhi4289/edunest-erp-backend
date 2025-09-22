--
-- PostgreSQL database dump
--

\restrict I63qPx5rFxebGuDZ65ePWxWnTXGV8cGOaT8qF8jXYFh4QX3uKl83h1FWt9YQYPv

-- Dumped from database version 15.14 (Homebrew)
-- Dumped by pg_dump version 15.14 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: assessments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.assessments (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    class_subject_id uuid NOT NULL,
    name text NOT NULL,
    assessment_date date,
    grading_type text DEFAULT 'marks'::text NOT NULL,
    max_marks integer,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    academic_year text,
    CONSTRAINT assessments_grading_type_check CHECK ((grading_type = ANY (ARRAY['marks'::text, 'grade'::text])))
);


ALTER TABLE public.assessments OWNER TO postgres;

--
-- Name: attendance; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.attendance (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    class_id uuid,
    date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    modified_at timestamp with time zone DEFAULT now() NOT NULL,
    is_present boolean DEFAULT true NOT NULL,
    edunest_id text NOT NULL
);


ALTER TABLE public.attendance OWNER TO postgres;

--
-- Name: class_subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class_subjects (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    class_id uuid NOT NULL,
    subject_id uuid NOT NULL,
    teacher_id uuid,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.class_subjects OWNER TO postgres;

--
-- Name: classes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.classes (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    name text NOT NULL,
    grade text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    section character varying(255)
);


ALTER TABLE public.classes OWNER TO postgres;

--
-- Name: subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subjects (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    name text NOT NULL,
    code text,
    description text,
    grading_type text DEFAULT 'marks'::text NOT NULL,
    max_marks integer,
    is_active boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT subjects_grading_type_check CHECK ((grading_type = ANY (ARRAY['marks'::text, 'grade'::text])))
);


ALTER TABLE public.subjects OWNER TO postgres;

--
-- Name: class_subject_view; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.class_subject_view AS
 SELECT c.id AS class_id,
    c.name AS class_name,
    c.grade,
    c.section,
    cs.id AS class_subject_id,
    s.id AS subject_id,
    s.name AS subject_name,
    s.code AS subject_code,
    s.grading_type,
    s.max_marks,
    cs.created_at,
    cs.updated_at
   FROM ((public.classes c
     LEFT JOIN public.class_subjects cs ON ((cs.class_id = c.id)))
     LEFT JOIN public.subjects s ON ((cs.subject_id = s.id)));


ALTER TABLE public.class_subject_view OWNER TO postgres;

--
-- Name: communications; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.communications (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    title character varying(255) NOT NULL,
    type character varying(50) NOT NULL,
    content text NOT NULL,
    status character varying(50) DEFAULT 'Active'::character varying,
    created_by uuid NOT NULL,
    modified_by uuid,
    created_at timestamp with time zone DEFAULT now(),
    modified_at timestamp with time zone DEFAULT now()
);


ALTER TABLE public.communications OWNER TO postgres;

--
-- Name: fee_admin; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fee_admin (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    class_id uuid NOT NULL,
    annual_fee numeric(12,2) DEFAULT 0 NOT NULL,
    monthly_fee numeric(12,2) DEFAULT 0 NOT NULL,
    admission_fee numeric(12,2) DEFAULT 0 NOT NULL,
    transport_fee numeric(12,2) DEFAULT 0 NOT NULL,
    examination_fee numeric(12,2) DEFAULT 0 NOT NULL,
    library_fee numeric(12,2) DEFAULT 0 NOT NULL,
    sports_fee numeric(12,2) DEFAULT 0 NOT NULL,
    miscellaneous_fee numeric(12,2) DEFAULT 0 NOT NULL,
    total_annual_fee numeric(12,2) GENERATED ALWAYS AS (((((((annual_fee + admission_fee) + transport_fee) + examination_fee) + library_fee) + sports_fee) + miscellaneous_fee)) STORED,
    is_active boolean DEFAULT true NOT NULL,
    academic_year character varying(10) DEFAULT (EXTRACT(year FROM CURRENT_DATE))::character varying NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT fee_admin_nonnegative_chk CHECK (((annual_fee >= (0)::numeric) AND (monthly_fee >= (0)::numeric) AND (admission_fee >= (0)::numeric) AND (transport_fee >= (0)::numeric) AND (examination_fee >= (0)::numeric) AND (library_fee >= (0)::numeric) AND (sports_fee >= (0)::numeric) AND (miscellaneous_fee >= (0)::numeric)))
);


ALTER TABLE public.fee_admin OWNER TO postgres;

--
-- Name: TABLE fee_admin; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.fee_admin IS 'Fee structure configuration for each class/grade';


--
-- Name: COLUMN fee_admin.class_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.fee_admin.class_id IS 'Reference to classes table';


--
-- Name: COLUMN fee_admin.total_annual_fee; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.fee_admin.total_annual_fee IS 'Computed total of all fees except monthly fee';


--
-- Name: COLUMN fee_admin.academic_year; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.fee_admin.academic_year IS 'Academic year for the fee structure';


--
-- Name: fees_collection; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fees_collection (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    student_edunest_id text NOT NULL,
    date_of_collection date NOT NULL,
    fee_collected numeric(12,2) DEFAULT 0 NOT NULL,
    fee_waived numeric(12,2) DEFAULT 0 NOT NULL,
    waiver_reason text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    modified_date timestamp with time zone DEFAULT now() NOT NULL,
    class_id uuid
);


ALTER TABLE public.fees_collection OWNER TO postgres;

--
-- Name: homework; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.homework (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    class_subject_id uuid NOT NULL,
    assigned_date date DEFAULT CURRENT_DATE NOT NULL,
    due_date date NOT NULL,
    details text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by_id uuid NOT NULL,
    CONSTRAINT homework_details_check CHECK ((length(details) <= 5000))
);


ALTER TABLE public.homework OWNER TO postgres;

--
-- Name: staff; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.staff (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    staff_id text NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    middle_name text,
    gender text,
    dob date,
    personal_email text,
    official_email text,
    phone text,
    role text NOT NULL,
    joining_date date NOT NULL,
    exit_date date,
    status text DEFAULT 'Active'::text NOT NULL,
    address_line1 text,
    address_line2 text,
    city text,
    state text,
    zip text,
    country text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT staff_check CHECK (((exit_date IS NULL) OR (exit_date >= joining_date))),
    CONSTRAINT staff_gender_check CHECK ((gender = ANY (ARRAY['Male'::text, 'Female'::text, 'Other'::text]))),
    CONSTRAINT staff_status_check CHECK ((status = ANY (ARRAY['Active'::text, 'On Leave'::text, 'Terminated'::text, 'Retired'::text])))
);


ALTER TABLE public.staff OWNER TO postgres;

--
-- Name: student_marks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.student_marks (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    assessment_id uuid NOT NULL,
    edunest_id text NOT NULL,
    marks_obtained numeric(6,2),
    grade_awarded text,
    remarks text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.student_marks OWNER TO postgres;

--
-- Name: students; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.students (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    status text DEFAULT 'Active'::text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    edunest_id text,
    date_of_birth date,
    father_name text,
    father_email text,
    mother_name text,
    mother_email text,
    created_by text,
    modified_at timestamp with time zone,
    modified_by text,
    admission_number text,
    phone_number text,
    secondary_phone_number text,
    email text,
    address_line1 text,
    address_line2 text,
    city text,
    state text,
    zip text,
    country text,
    classid uuid
);


ALTER TABLE public.students OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    user_id text NOT NULL,
    password_hash text NOT NULL,
    role text DEFAULT 'Admin'::text NOT NULL,
    first_login_completed boolean DEFAULT false NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    name text
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: v_assessments_overview; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.v_assessments_overview AS
 SELECT c.grade,
    c.section,
    a.academic_year,
    a.id AS assessment_id,
    a.name AS assessment_name,
    s.name AS subject_name,
    a.grading_type,
    a.max_marks
   FROM (((public.assessments a
     JOIN public.class_subjects cs ON ((a.class_subject_id = cs.id)))
     JOIN public.classes c ON ((cs.class_id = c.id)))
     JOIN public.subjects s ON ((cs.subject_id = s.id)))
  ORDER BY c.grade, c.section, a.academic_year, s.name, a.name;


ALTER TABLE public.v_assessments_overview OWNER TO postgres;

--
-- Name: v_class_subjects; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.v_class_subjects AS
 SELECT cs.id AS class_subject_id,
    c.id AS class_id,
    c.grade,
    c.section,
    s.id AS subject_id,
    s.name AS subject_name,
    s.code AS subject_code,
    s.grading_type,
    s.max_marks,
    cs.created_at,
    cs.updated_at
   FROM ((public.class_subjects cs
     LEFT JOIN public.classes c ON ((cs.class_id = c.id)))
     LEFT JOIN public.subjects s ON ((cs.subject_id = s.id)));


ALTER TABLE public.v_class_subjects OWNER TO postgres;

--
-- Name: v_student_marks_academic_year; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.v_student_marks_academic_year AS
 SELECT s.edunest_id,
    s.first_name,
    s.last_name,
    c.grade,
    c.section,
    a.academic_year,
    sub.name AS subject_name,
    a.name AS assessment_name,
    a.grading_type,
    a.max_marks,
    sm.marks_obtained,
    sm.grade_awarded,
    sm.remarks,
    a.assessment_date,
    sm.created_at AS mark_created_at,
    sm.updated_at AS mark_updated_at,
    a.id AS assessment_id
   FROM (((((public.student_marks sm
     JOIN public.assessments a ON ((sm.assessment_id = a.id)))
     JOIN public.class_subjects cs ON ((a.class_subject_id = cs.id)))
     JOIN public.classes c ON ((cs.class_id = c.id)))
     JOIN public.subjects sub ON ((cs.subject_id = sub.id)))
     JOIN public.students s ON ((sm.edunest_id = s.edunest_id)))
  ORDER BY s.edunest_id, a.academic_year, sub.name, a.name;


ALTER TABLE public.v_student_marks_academic_year OWNER TO postgres;

--
-- Name: assessments assessments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.assessments
    ADD CONSTRAINT assessments_pkey PRIMARY KEY (id);


--
-- Name: attendance attendance_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_pkey PRIMARY KEY (id);


--
-- Name: class_subjects class_subjects_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_subjects
    ADD CONSTRAINT class_subjects_pkey PRIMARY KEY (id);


--
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- Name: communications communications_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.communications
    ADD CONSTRAINT communications_pkey PRIMARY KEY (id);


--
-- Name: fee_admin fee_admin_class_year_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fee_admin
    ADD CONSTRAINT fee_admin_class_year_unique UNIQUE (class_id, academic_year);


--
-- Name: fee_admin fee_admin_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fee_admin
    ADD CONSTRAINT fee_admin_pkey PRIMARY KEY (id);


--
-- Name: fees_collection fees_collection_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fees_collection
    ADD CONSTRAINT fees_collection_pkey PRIMARY KEY (id);


--
-- Name: homework homework_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.homework
    ADD CONSTRAINT homework_pkey PRIMARY KEY (id);


--
-- Name: staff staff_official_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT staff_official_email_key UNIQUE (official_email);


--
-- Name: staff staff_personal_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT staff_personal_email_key UNIQUE (personal_email);


--
-- Name: staff staff_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT staff_pkey PRIMARY KEY (id);


--
-- Name: staff staff_staff_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT staff_staff_id_key UNIQUE (staff_id);


--
-- Name: student_marks student_marks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_marks
    ADD CONSTRAINT student_marks_pkey PRIMARY KEY (id);


--
-- Name: students students_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- Name: subjects subjects_code_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_code_key UNIQUE (code);


--
-- Name: subjects subjects_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_name_key UNIQUE (name);


--
-- Name: subjects subjects_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_pkey PRIMARY KEY (id);


--
-- Name: class_subjects uq_class_subject; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_subjects
    ADD CONSTRAINT uq_class_subject UNIQUE (class_id, subject_id);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (user_id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: student_marks ux_student_marks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_marks
    ADD CONSTRAINT ux_student_marks UNIQUE (assessment_id, edunest_id);


--
-- Name: idx_fee_admin_academic_year; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fee_admin_academic_year ON public.fee_admin USING btree (academic_year);


--
-- Name: idx_fee_admin_active; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fee_admin_active ON public.fee_admin USING btree (is_active);


--
-- Name: idx_fee_admin_class_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fee_admin_class_id ON public.fee_admin USING btree (class_id);


--
-- Name: idx_homework_assigned_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_homework_assigned_date ON public.homework USING btree (assigned_date);


--
-- Name: idx_homework_class_subject; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_homework_class_subject ON public.homework USING btree (class_subject_id);


--
-- Name: idx_homework_due_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_homework_due_date ON public.homework USING btree (due_date);


--
-- Name: idx_staff_staff_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_staff_staff_id ON public.staff USING btree (staff_id);


--
-- Name: idx_students_classid; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_students_classid ON public.students USING btree (classid);


--
-- Name: ix_fees_collection_class_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_fees_collection_class_id ON public.fees_collection USING btree (class_id);


--
-- Name: ix_fees_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_fees_date ON public.fees_collection USING btree (date_of_collection);


--
-- Name: ix_fees_student; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_fees_student ON public.fees_collection USING btree (student_edunest_id);


--
-- Name: ix_student_marks_assessment; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_student_marks_assessment ON public.student_marks USING btree (assessment_id);


--
-- Name: ix_student_marks_student; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_student_marks_student ON public.student_marks USING btree (edunest_id);


--
-- Name: ux_students_edunest_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_students_edunest_id ON public.students USING btree (edunest_id);


--
-- Name: assessments assessments_class_subject_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.assessments
    ADD CONSTRAINT assessments_class_subject_id_fkey FOREIGN KEY (class_subject_id) REFERENCES public.class_subjects(id) ON DELETE CASCADE;


--
-- Name: attendance attendance_class_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_class_id_fkey FOREIGN KEY (class_id) REFERENCES public.classes(id) ON DELETE CASCADE;


--
-- Name: attendance attendance_edunest_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_edunest_id_fkey FOREIGN KEY (edunest_id) REFERENCES public.students(edunest_id) ON DELETE CASCADE;


--
-- Name: fee_admin fee_admin_class_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fee_admin
    ADD CONSTRAINT fee_admin_class_fk FOREIGN KEY (class_id) REFERENCES public.classes(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fees_collection fees_student_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fees_collection
    ADD CONSTRAINT fees_student_fk FOREIGN KEY (student_edunest_id) REFERENCES public.students(edunest_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: class_subjects fk_class_subjects_class; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_subjects
    ADD CONSTRAINT fk_class_subjects_class FOREIGN KEY (class_id) REFERENCES public.classes(id) ON DELETE CASCADE;


--
-- Name: class_subjects fk_class_subjects_subject; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_subjects
    ADD CONSTRAINT fk_class_subjects_subject FOREIGN KEY (subject_id) REFERENCES public.subjects(id) ON DELETE CASCADE;


--
-- Name: communications fk_created_by; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.communications
    ADD CONSTRAINT fk_created_by FOREIGN KEY (created_by) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: fees_collection fk_fees_collection_class; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fees_collection
    ADD CONSTRAINT fk_fees_collection_class FOREIGN KEY (class_id) REFERENCES public.classes(id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: communications fk_modified_by; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.communications
    ADD CONSTRAINT fk_modified_by FOREIGN KEY (modified_by) REFERENCES public.users(id) ON DELETE SET NULL;


--
-- Name: students fk_students_classid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.students
    ADD CONSTRAINT fk_students_classid FOREIGN KEY (classid) REFERENCES public.classes(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: homework homework_class_subject_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.homework
    ADD CONSTRAINT homework_class_subject_id_fkey FOREIGN KEY (class_subject_id) REFERENCES public.class_subjects(id) ON DELETE CASCADE;


--
-- Name: homework homework_created_by_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.homework
    ADD CONSTRAINT homework_created_by_id_fkey FOREIGN KEY (created_by_id) REFERENCES public.users(id) ON DELETE SET NULL;


--
-- Name: student_marks student_marks_assessment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_marks
    ADD CONSTRAINT student_marks_assessment_id_fkey FOREIGN KEY (assessment_id) REFERENCES public.assessments(id) ON DELETE CASCADE;


--
-- Name: student_marks student_marks_edunest_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_marks
    ADD CONSTRAINT student_marks_edunest_id_fkey FOREIGN KEY (edunest_id) REFERENCES public.students(edunest_id) ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: pg_database_owner
--

GRANT USAGE ON SCHEMA public TO edunest_app;


--
-- Name: TABLE assessments; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.assessments TO edunest_app;


--
-- Name: TABLE attendance; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.attendance TO edunest_app;


--
-- Name: TABLE class_subjects; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.class_subjects TO edunest_app;


--
-- Name: TABLE classes; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.classes TO edunest_app;


--
-- Name: TABLE subjects; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.subjects TO edunest_app;


--
-- Name: TABLE class_subject_view; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.class_subject_view TO edunest_app;


--
-- Name: TABLE communications; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.communications TO edunest_app;


--
-- Name: TABLE fee_admin; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.fee_admin TO edunest_app;


--
-- Name: TABLE fees_collection; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.fees_collection TO edunest_app;


--
-- Name: TABLE homework; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.homework TO edunest_app;


--
-- Name: TABLE staff; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.staff TO edunest_app;


--
-- Name: TABLE student_marks; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.student_marks TO edunest_app;


--
-- Name: TABLE students; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.students TO edunest_app;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.users TO edunest_app;


--
-- Name: TABLE v_assessments_overview; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.v_assessments_overview TO edunest_app;


--
-- Name: TABLE v_class_subjects; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.v_class_subjects TO edunest_app;


--
-- Name: TABLE v_student_marks_academic_year; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.v_student_marks_academic_year TO edunest_app;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT SELECT,INSERT,DELETE,UPDATE ON TABLES  TO edunest_app;


--
-- PostgreSQL database dump complete
--

\unrestrict I63qPx5rFxebGuDZ65ePWxWnTXGV8cGOaT8qF8jXYFh4QX3uKl83h1FWt9YQYPv

